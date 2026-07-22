using System.Linq;
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
    /// <param name="registerCustomerUsecase">顧客アカウント登録Usecase</param>
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
        [FromBody] RegisterCustomerRequest? request)
    {
        // リクエストボディが存在しない場合
        if (request is null)
        {
            return BadRequest(new
            {
                message = "リクエストボディが正しくありません。"
            });
        }

        // RegisterCustomerRequestのDataAnnotationsで検出された
        // 詳細な入力値エラーメッセージを取得する。
        // 複数エラーがある場合は最初の1件を返す。
        if (!ModelState.IsValid)
        {
            var validationMessage = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .FirstOrDefault(message =>
                    !string.IsNullOrWhiteSpace(message))
                ?? "入力値に不備があります。";

            return BadRequest(new
            {
                message = validationMessage
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
            // アカウント名・メールアドレス・電話番号などが
            // 既に登録されている場合
            return Conflict(new
            {
                message = ex.Message
            });
        }
        catch (DomainException ex)
        {
            // Domain層で検出した具体的な入力値ルールを返す。
            var errorMessage = string.IsNullOrWhiteSpace(ex.Message)
                ? "入力値に不備があります。"
                : ex.Message;

            return BadRequest(new
            {
                message = errorMessage
            });
        }
    }
}