using System.Security.Claims;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 注文明細取得APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/orders")]
[Tags("注文明細")]
//石原:変更 注文明細取得APIをJWT認証必須に変更
[Authorize]
public class GetOrderDetailsController : ControllerBase
{
    private readonly IGetOrderDetailsUsecase
        _getOrderDetailsUsecase;

    private readonly GetOrderDetailsViewModelAdapter
        _getOrderDetailsViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getOrderDetailsUsecase">
    /// 注文明細取得Usecase
    /// </param>
    /// <param name="getOrderDetailsViewModelAdapter">
    /// 注文明細ViewModelAdapter
    /// </param>
    public GetOrderDetailsController(
        IGetOrderDetailsUsecase getOrderDetailsUsecase,
        GetOrderDetailsViewModelAdapter
            getOrderDetailsViewModelAdapter)
    {
        _getOrderDetailsUsecase =
            getOrderDetailsUsecase;

        _getOrderDetailsViewModelAdapter =
            getOrderDetailsViewModelAdapter;
    }

    /// <summary>
    /// 注文UUIDに紐づく注文明細を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>注文明細一覧</returns>
    [HttpGet("{orderUuid}")]
    public async Task<
        ActionResult<List<OrderDetailResponse>>>
        GetOrderDetailsAsync(
            [FromRoute] string orderUuid)
    {
        if (!Guid.TryParse(
                orderUuid,
                out var parsedOrderUuid))
        {
            return BadRequest(new
            {
                message =
                    "注文UUIDの形式が正しくありません。"
            });
        }

        //石原:追加 JWTからログイン中の顧客UUIDを取得
        var customerUuid =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        //石原:追加 認証済みでも顧客UUIDを取得できない場合は401を返す
        if (string.IsNullOrWhiteSpace(
                customerUuid))
        {
            return Unauthorized(new
            {
                message =
                    "顧客情報を取得できませんでした。"
            });
        }

        //石原:変更 注文UUIDと顧客UUIDを渡して本人の注文だけを取得
        var orderDetails =
            await _getOrderDetailsUsecase
                .ExecuteAsync(
                    parsedOrderUuid.ToString(),
                    customerUuid);

        if (orderDetails.Count == 0)
        {
            return NotFound(new
            {
                message =
                    "指定した注文の明細が見つかりませんでした。"
            });
        }

        var responses =
            await _getOrderDetailsViewModelAdapter
                .ConvertAsync(orderDetails);

        return Ok(responses);
    }
}