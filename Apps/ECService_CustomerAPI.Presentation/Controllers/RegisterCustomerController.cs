using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECService_CustomerAPI.Presentation.Controllers;

/// <summary>
/// 顧客アカウント登録APIを提供するController
/// </summary>
[ApiController]
[Route("/api/customer/accounts")]
[Tags("顧客アカウント")]
public class RegisterCustomerController : ControllerBase
{
    private readonly IRegisterCustomerUsecase _registerCustomerUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="registerCustomerUsecase">
    /// 顧客アカウント登録Usecase
    /// </param>
    public RegisterCustomerController(
        IRegisterCustomerUsecase registerCustomerUsecase)
    {
        _registerCustomerUsecase = registerCustomerUsecase;
    }

    /// <summary>
    /// 顧客アカウントを登録する
    /// </summary>
    /// <param name="request">顧客アカウント登録情報</param>
    /// <returns>登録結果</returns>
    [HttpPost]
    public async Task<IActionResult> RegisterCustomerAsync(
        [FromBody] RegisterCustomerRequest request)
    {

        if (request is null)
        {
            return BadRequest(new
            {
                message = "リクエストボディが正しくありません。"
            });
        }
        var hasEmptyRequiredField =
            string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.NameKana) ||
            string.IsNullOrWhiteSpace(request.Address1) ||
            string.IsNullOrWhiteSpace(request.PhoneNumber) ||
            string.IsNullOrWhiteSpace(request.MailAddress) ||
            string.IsNullOrWhiteSpace(request.AccountName) ||
            string.IsNullOrWhiteSpace(request.Password);

        if (hasEmptyRequiredField)
        {
            return BadRequest(new
            {
                message = "未入力項目が存在しています。"
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "入力値に不備があります。"
            });
        }

        try
        {
            var customerUuid =
                await _registerCustomerUsecase.ExecuteAsync(
                    (
                        request.Name,
                        request.NameKana,
                        request.Address1,
                        request.Address2,
                        request.PhoneNumber,
                        request.MailAddress,
                        request.AccountName,
                        request.Password
                    ));

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    customerUuid,
                    message = "アカウント登録が完了しました。"
                });
        }
        catch (ConflictException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (DomainException)
        {
            return BadRequest(new
            {
                message = "入力値に不備があります。"
            });
        }
    }
}