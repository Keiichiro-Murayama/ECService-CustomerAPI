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
// [Authorize]
public class GetOrderDetailsController : ControllerBase
{
    private readonly IGetOrderDetailsUsecase _getOrderDetailsUsecase;
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
        _getOrderDetailsUsecase = getOrderDetailsUsecase;
        _getOrderDetailsViewModelAdapter =
            getOrderDetailsViewModelAdapter;
    }

    /// <summary>
    /// 注文UUIDに紐づく注文明細を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>注文明細一覧</returns>
    [HttpGet("{orderUuid}")]
    public async Task<ActionResult<List<OrderDetailResponse>>>
        GetOrderDetailsAsync(
            [FromRoute] string orderUuid)
    {
        //石原:追加
        if (!Guid.TryParse(orderUuid, out var parsedOrderUuid))
        {
            return BadRequest(new
            {
                message = "注文UUIDの形式が正しくありません。"
            });
        }

        var orderDetails =
            await _getOrderDetailsUsecase.ExecuteAsync(
                parsedOrderUuid.ToString());

        if (orderDetails.Count == 0)
        {
            return NotFound(new
            {
                message = "指定した注文の明細が見つかりませんでした。"
            });
        }

        var responses =
            await _getOrderDetailsViewModelAdapter.ConvertAsync(
                orderDetails);

        return Ok(responses);
    }
}