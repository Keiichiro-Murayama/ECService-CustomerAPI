using System.Security.Claims;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 購入履歴取得APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/orders")]
[Tags("購入履歴")]
[Authorize]
public class GetOrderHistoryController : ControllerBase
{
    private readonly IGetOrderHistoriesUsecase _getOrderHistoriesUsecase;
    private readonly GetOrderHistoriesViewModelAdapter
        _getOrderHistoriesViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getOrderHistoriesUsecase">
    /// 購入履歴取得Usecase
    /// </param>
    /// <param name="getOrderHistoriesViewModelAdapter">
    /// 購入履歴ViewModelAdapter
    /// </param>
    public GetOrderHistoryController(
        IGetOrderHistoriesUsecase getOrderHistoriesUsecase,
        GetOrderHistoriesViewModelAdapter
            getOrderHistoriesViewModelAdapter)
    {
        _getOrderHistoriesUsecase = getOrderHistoriesUsecase;
        _getOrderHistoriesViewModelAdapter =
            getOrderHistoriesViewModelAdapter;
    }

    /// <summary>
    /// ログイン中の顧客の購入履歴を取得する
    /// </summary>
    /// <returns>購入履歴一覧</returns>
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>>
        GetOrderHistoriesAsync()
    {
        var customerUuid =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(customerUuid))
        {
            return Unauthorized(new
            {
                message = "顧客情報を取得できませんでした。"
            });
        }

        var orders =
            await _getOrderHistoriesUsecase.ExecuteAsync(
                customerUuid);

        var responses =
            _getOrderHistoriesViewModelAdapter.ConvertAsync(
                orders);

        return Ok(responses);
    }
}