using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.UnitOfWorks;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 商品購入処理を行うユースケース
/// </summary>
public class PurchaseUsecase : IPurchaseUsecase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrdersRepository _ordersRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="customerRepository">
    /// 顧客リポジトリ
    /// </param>
    /// <param name="paymentMethodRepository">
    /// 支払い方法リポジトリ
    /// </param>
    /// <param name="productRepository">
    /// 商品リポジトリ
    /// </param>
    /// <param name="ordersRepository">
    /// 注文リポジトリ
    /// </param>
    /// <param name="unitOfWork">
    /// トランザクション管理
    /// </param>
    public PurchaseUsecase(
        ICustomerRepository customerRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IProductRepository productRepository,
        IOrdersRepository ordersRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _productRepository = productRepository;
        _ordersRepository = ordersRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 商品の購入を確定する
    /// </summary>
    /// <param name="customerUuid">
    /// JWTから取得した顧客UUID
    /// </param>
    /// <param name="paymentMethodId">
    /// 支払い方法ID
    /// </param>
    /// <param name="items">
    /// 購入商品のUUIDと数量
    /// </param>
    /// <returns>
    /// 登録した注文の注文UUID
    /// </returns>
    public async Task<string> ExecuteAsync(
        string customerUuid,
        int paymentMethodId,
        List<(string ProductUuid, int Quantity)> items)
    {
        /*
         * 1. 購入商品が指定されているか確認する
         */
        if (items == null || items.Count == 0)
        {
            throw new DomainException(
                "購入する商品を1件以上指定してください。",
                nameof(items));
        }

        /*
         * 2. OrderDetailを生成する
         *
         * OrderDetail.Create内で以下を検証する。
         * ・商品UUIDが入力されていること
         * ・商品UUIDがUUID形式であること
         * ・購入数量が1以上であること
         *
         * DB更新前に検証することで、不正な数量による
         * 在庫更新を防止する。
         */
        var orderDetails = new List<OrderDetail>();

        foreach (var item in items)
        {
            var orderDetail = OrderDetail.Create(
                item.ProductUuid,
                item.Quantity);

            orderDetails.Add(orderDetail);
        }

        /*
         * 3. 顧客を検索する
         */
        var customer =
            await _customerRepository.FindByUuidAsync(customerUuid);

        if (customer == null)
        {
            throw new NotFoundException(
                "顧客が見つかりません。");
        }

        /*
         * 4. 支払い方法一覧を取得する
         */
        var paymentMethods =
            await _paymentMethodRepository.SelectAllAsync();

        /*
         * 5. 指定された支払い方法が存在するか確認する
         */
        var paymentMethodExists =
            paymentMethods.Any(
                paymentMethod =>
                    paymentMethod.Id == paymentMethodId);

        if (!paymentMethodExists)
        {
            throw new NotFoundException(
                "指定されたリソースが見つかりません。");
        }

        var amountTotal = 0;
        var transactionStarted = false;

        try
        {
            /*
             * 6. トランザクションを開始する
             */
            await _unitOfWork.BeginTransactionAsync();
            transactionStarted = true;

            /*
             * 7. 購入商品ごとに、
             *    価格取得・在庫更新・合計金額計算を行う
             */
            foreach (var orderDetail in orderDetails)
            {
                int price;

                /*
                 * 7-1. 商品価格を取得する
                 */
                try
                {
                    price =
                        await _productRepository
                            .SelectPriceByProductUuidAsync(
                                orderDetail.ProductUuid);
                }
                catch (InternalException ex)
                    when (IsProductNotFoundException(ex))
                {
                    /*
                     * 現在のProductRepositoryでは、
                     * 商品が存在しない場合にInternalExceptionが
                     * 発生するため、404用の例外へ変換する。
                     */
                    throw new NotFoundException(
                        $"商品UUID「{orderDetail.ProductUuid}」の商品が見つかりません。",
                        ex);
                }

                /*
                 * 7-2. 商品在庫を悲観的ロックし、
                 *      購入数量分だけ減らす
                 */
                try
                {
                    await _productRepository
                        .UpdateProductStockAsync(
                            orderDetail.ProductUuid,
                            orderDetail.Count);
                }
                catch (InternalException ex)
                    when (IsStockShortageException(ex))
                {
                    /*
                     * 在庫不足を業務上の入力エラーへ変換する。
                     */
                    throw new DomainException(
                        $"商品UUID「{orderDetail.ProductUuid}」の在庫が不足しています。",
                        "quantity");
                }
                catch (InternalException ex)
                    when (IsProductNotFoundException(ex))
                {
                    /*
                     * 価格取得後に商品が削除された場合なども考慮し、
                     * 在庫更新時の商品なしも404用の例外へ変換する。
                     */
                    throw new NotFoundException(
                        $"商品UUID「{orderDetail.ProductUuid}」の商品が見つかりません。",
                        ex);
                }

                /*
                 * 7-3. 合計金額を計算する
                 *
                 * checkedを使用し、intの上限を超えた場合は
                 * OverflowExceptionを発生させる。
                 */
                amountTotal = checked(
                    amountTotal +
                    price * orderDetail.Count);
            }

            /*
             * 8. 注文ドメインオブジェクトを生成する
             *
             * Orders.Create内部で以下が設定される。
             * ・注文UUIDの新規発行
             * ・初期注文ステータスID = 1（注文済み）
             */
            var order = Orders.Create(
                amountTotal,
                customerUuid,
                paymentMethodId,
                orderDetails);

            /*
             * 9. 注文と注文明細を登録する
             */
            var orderUuid =
                await _ordersRepository.CreateAsync(order);

            /*
             * 10. トランザクションをコミットする
             */
            await _unitOfWork.CommitAsync();
            transactionStarted = false;

            /*
             * 11. 登録した注文UUIDを返す
             */
            return orderUuid;
        }
        catch
        {
            /*
             * トランザクション開始後にエラーが発生した場合、
             * 在庫更新と注文登録を取り消す。
             */
            if (transactionStarted)
            {
                await _unitOfWork.RollbackAsync();
            }

            /*
             * 発生した例外をControllerまたは
             * 例外処理Middlewareへ再送出する。
             */
            throw;
        }
    }

    /// <summary>
    /// 商品が存在しないことを表す例外か判定する
    /// </summary>
    /// <param name="exception">
    /// ProductRepositoryから発生した内部例外
    /// </param>
    /// <returns>
    /// 商品が存在しない場合はtrue
    /// </returns>
    private static bool IsProductNotFoundException(
        InternalException exception)
    {
        return exception.Message.Contains(
            "が見つかりません。",
            StringComparison.Ordinal);
    }

    /// <summary>
    /// 在庫不足を表す例外か判定する
    /// </summary>
    /// <param name="exception">
    /// ProductRepositoryから発生した内部例外
    /// </param>
    /// <returns>
    /// 在庫不足の場合はtrue
    /// </returns>
    private static bool IsStockShortageException(
        InternalException exception)
    {
        return exception.Message.Contains(
            "在庫が不足しています。",
            StringComparison.Ordinal);
    }
}