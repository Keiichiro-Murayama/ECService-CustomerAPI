using System.Security.Claims;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 顧客アカウント情報取得APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/profile")]
[Tags("顧客アカウント")]
[Authorize]
public class GetCustomerInfoController : ControllerBase
{
    private readonly IGetCustomerInfoUsecase _getCustomerInfoUsecase;
    private readonly GetCustomerInfoViewModelAdapter
        _getCustomerInfoViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getCustomerInfoUsecase">
    /// 顧客アカウント情報取得Usecase
    /// </param>
    /// <param name="getCustomerInfoViewModelAdapter">
    /// 顧客情報ViewModelAdapter
    /// </param>
    public GetCustomerInfoController(
        IGetCustomerInfoUsecase getCustomerInfoUsecase,
        GetCustomerInfoViewModelAdapter
            getCustomerInfoViewModelAdapter)
    {
        _getCustomerInfoUsecase = getCustomerInfoUsecase;
        _getCustomerInfoViewModelAdapter =
            getCustomerInfoViewModelAdapter;
    }

    /// <summary>
    /// ログイン中の顧客アカウント情報を取得する
    /// </summary>
    /// <returns>顧客アカウント情報</returns>
    [HttpGet]
    public async Task<ActionResult<CustomerInfoResponse>>
        GetCustomerInfoAsync()
    {
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

        var customer =
            await _getCustomerInfoUsecase.ExecuteAsync(
                customerUuid);

        if (customer is null)
        {
            return NotFound(new
            {
                message = "顧客情報が見つかりません。"
            });
        }

        var response =
            _getCustomerInfoViewModelAdapter.ConvertAsync(
                customer);

        return Ok(response);
    }
}