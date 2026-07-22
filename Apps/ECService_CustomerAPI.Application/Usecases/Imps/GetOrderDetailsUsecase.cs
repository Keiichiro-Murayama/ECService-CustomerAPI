using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 注文明細取得Usecaseの実装
/// </summary>
public class GetOrderDetailsUsecase : IGetOrderDetailsUsecase
{
    private readonly IOrdersRepository _ordersRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ordersRepository">注文Repository</param>
    public GetOrderDetailsUsecase(
        IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    /// <summary>
    /// 注文UUIDと顧客UUIDに紐づく注文明細を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <param name="customerUuid">ログイン中の顧客UUID</param>
    /// <returns>注文明細一覧</returns>
    //石原:変更 他顧客の注文明細取得を防ぐため、顧客UUIDをRepositoryへ渡す
    public async Task<List<OrderDetail>> ExecuteAsync(
        string orderUuid,
        string customerUuid)
    {
        return await _ordersRepository
            .SelectOrderDetailsByOrderUuidAsync(
                orderUuid,
                customerUuid);
    }
}