using System.Security.Claims;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 商品購入に関するAPIを提供する
/// </summary>
[ApiController]
[Route("/api/customer")]
[Tags("商品購入")]
[Authorize]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseUsecase _purchaseUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="purchaseUsecase">
    /// 商品購入ユースケース
    /// </param>
    public PurchaseController(
        IPurchaseUsecase purchaseUsecase)
    {
        _purchaseUsecase = purchaseUsecase;
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
    public async Task<ActionResult<PurchaseResponse>> Purchase(
        [FromBody] PurchaseRequest? model)
    {
        /*
         * 1. リクエストボディが送信されているか確認する
         */
        if (model == null)
        {
            return BadRequest(new
            {
                message = "購入情報を入力してください。"
            });
        }

        /*
         * 2. JWTから顧客UUIDを取得する
         *
         * JwtTokenProviderではsubクレームに
         * CustomerUuidを設定している。
         *
         * JWTのクレーム変換によって
         * NameIdentifierへ変換される場合も考慮する。
         */
        var customerUuid =
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        /*
         * Authorize属性により通常は認証処理で拒否されるが、
         * JWT内に顧客UUIDが存在しない場合も確認する。
         */
        if (string.IsNullOrWhiteSpace(customerUuid))
        {
            return Unauthorized(new
            {
                error = "Unauthorized",
                message = "認証が必要です。ログインしてください。"
            });
        }

        /*
         * 3. 購入商品一覧をUsecaseの引数形式へ変換する
         */
        var items =
            model.Items?
                .Select(item =>
                    (
                        ProductUuid: item.ProductUuid,
                        Quantity: item.Quantity
                    ))
                .ToList()
            ?? new List<(string ProductUuid, int Quantity)>();

        /*
         * 4. 商品購入ユースケースを実行する
         *
         * DomainExceptionやNotFoundExceptionなどは、
         * 後で例外処理MiddlewareがHTTPレスポンスへ変換する。
         */
        var orderUuid =
            await _purchaseUsecase.ExecuteAsync(
                customerUuid,
                model.PaymentMethodId,
                items);

        /*
         * 5. 購入結果を返す
         */
        var response = new PurchaseResponse
        {
            OrderUuid = orderUuid,
            Message = "購入が完了しました。"
        };

        return Ok(response);
    }
}