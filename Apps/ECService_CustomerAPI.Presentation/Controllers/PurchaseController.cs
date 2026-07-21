using System.Security.Claims;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 商品購入に関するAPIを提供する
/// </summary>
[ApiController]
[Route("api/customer")]
[Tags("商品購入")]
[Authorize]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseUsecase _purchaseUsecase;
    private readonly ILogger<PurchaseController> _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="purchaseUsecase">
    /// 商品購入ユースケース
    /// </param>
    /// <param name="logger">
    /// ログ出力
    /// </param>
    public PurchaseController(
        IPurchaseUsecase purchaseUsecase,
        ILogger<PurchaseController> logger)
    {
        _purchaseUsecase = purchaseUsecase;
        _logger = logger;
    }

    /// <summary>
    /// 商品の購入を確定する
    /// </summary>
    /// <param name="model">
    /// 支払い方法と購入商品一覧
    /// </param>
    /// <returns>
    /// 注文UUIDと購入完了メッセージ
    /// </returns>
    [HttpPost("orders")]
    [ProducesResponseType(
        typeof(PurchaseResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PurchaseResponse>> PurchaseAsync(
        [FromBody] PurchaseRequest? model)
    {
        /*
         * 1. リクエストの必須項目を確認する
         */
        if (model == null
            || string.IsNullOrWhiteSpace(model.PaymentMethodId)
            || model.Items == null
            || model.Items.Count == 0)
        {
            return BadRequest(new
            {
                message =
                    "支払い方法、または購入商品を選択してください。"
            });
        }

        /*
         * 2. 支払い方法IDを数値へ変換する
         */
        if (!int.TryParse(
                model.PaymentMethodId,
                out var paymentMethodId)
            || paymentMethodId < 1)
        {
            return BadRequest(new
            {
                message =
                    "支払い方法、または購入商品を選択してください。"
            });
        }

        /*
         * 3. 商品UUIDが未入力の商品がないか確認する
         */
        if (model.Items.Any(
                item =>
                    item == null
                    || string.IsNullOrWhiteSpace(
                        item.ProductUuid)))
        {
            return BadRequest(new
            {
                message =
                    "支払い方法、または購入商品を選択してください。"
            });
        }

        /*
         * 4. JWTから顧客UUIDを取得する
         *
         * JwtTokenProviderでは、subクレームに
         * CustomerUuidを設定している。
         *
         * JWTのクレーム変換によって
         * NameIdentifierになる場合も考慮する。
         */
        var customerUuid =
            User.FindFirst(
                JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

        /*
         * [Authorize]により通常はControllerへ到達する前に
         * 401となるが、JWT内に顧客UUIDが存在しない場合も確認する。
         */
        if (string.IsNullOrWhiteSpace(customerUuid))
        {
            return Unauthorized(new
            {
                message =
                    "認証が必要です。ログインしてください。"
            });
        }

        /*
         * 5. ViewModelをUsecaseの引数形式へ変換する
         */
        var items = model.Items
            .Select(item =>
                (
                    ProductUuid: item.ProductUuid,
                    Quantity: item.Quantity
                ))
            .ToList();

        try
        {
            /*
             * 6. 商品購入処理を実行する
             */
            var orderUuid =
                await _purchaseUsecase.ExecuteAsync(
                    customerUuid,
                    paymentMethodId,
                    items);

            /*
             * 7. 正常終了レスポンスを返す
             */
            var response = new PurchaseResponse
            {
                OrderUuid = orderUuid,
                Message = "購入が完了しました。"
            };

            return Ok(response);
        }
        catch (DomainException ex)
        {
            /*
             * UUID形式不正、数量不正、
             * 在庫不足など
             */
            _logger.LogWarning(
                ex,
                "商品購入処理で入力エラーが発生しました。");

            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (NotFoundException ex)
        {
            /*
             * 顧客、商品、支払い方法などが
             * 存在しない場合
             */
            _logger.LogWarning(
                ex,
                "商品購入処理で対象データが見つかりませんでした。");

            return NotFound(new
            {
                message =
                    "指定されたリソースが見つかりません。"
            });
        }
        catch (InternalException ex)
        {
            /*
             * DB処理などの内部エラー
             */
            _logger.LogError(
                ex,
                "商品購入処理で内部エラーが発生しました。");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "サーバー内部で予期せぬエラーが発生しました。"
                });
        }
        catch (Exception ex)
        {
            /*
             * 想定していない例外
             */
            _logger.LogError(
                ex,
                "商品購入処理で予期しないエラーが発生しました。");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "サーバー内部で予期せぬエラーが発生しました。"
                });
        }
    }
}