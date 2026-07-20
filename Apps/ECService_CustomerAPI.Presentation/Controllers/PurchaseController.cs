using System.Security.Claims;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<PurchaseResponse>> Purchase(
        [FromBody] PurchaseRequest? model)
    {
        /*
         * 1. リクエストボディを確認する
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
         * JWTのsubがNameIdentifierへ変換される場合と、
         * subのまま保持される場合の両方に対応する。
         */
        var customerUuid =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(customerUuid))
        {
            return Unauthorized(new
            {
                message = "認証が必要です。ログインしてください。"
            });
        }

        /*
         * 3. ViewModelをUsecaseの引数形式へ変換する
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

        try
        {
            /*
             * 4. 商品購入処理を実行する
             */
            var orderUuid =
                await _purchaseUsecase.ExecuteAsync(
                    customerUuid,
                    model.PaymentMethodId,
                    items);

            /*
             * 5. 正常終了レスポンスを返す
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
             * UUID形式不正、数量不正、商品未指定、
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
             * 顧客、商品、支払い方法が存在しない場合
             */
            _logger.LogWarning(
                ex,
                "商品購入処理で対象データが見つかりませんでした。");

            return NotFound(new
            {
                message = ex.Message
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