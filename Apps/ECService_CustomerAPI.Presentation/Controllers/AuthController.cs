using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ECService_CustomerAPI.Presentation.Controllers;
/// <summary>
/// 認証(ログイン・ログアウト)に関する API を提供する
/// </summary>
[ApiController]
[Route("/api/customer")]
[Tags("認証")]
public class AuthController : ControllerBase
{
    /// <summary>認証トークンを格納する Cookie のキー名</summary>
    private const string AuthCookieName = "access_token";

    private readonly ILoginUsecase _loginUsecase;

    public AuthController(
        ILoginUsecase loginUsecase)
    {
        _loginUsecase = loginUsecase;
    }

    /// <summary>
    /// ログインする
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            //APIの仕様上、バリデーションエラーは 400 Bad Request で返す
            return BadRequest(new
            {
                message = "入力値が不正です。",
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
            });
        }
        try
        {
            var result = await _loginUsecase.ExecuteAsync((model.EmailAddress, model.Password));

            AppendAuthCookie(result.AccessToken);

            var response = new TokenResponse
            {
                Token = result.AccessToken,
                CustomerUuid = result.Customer.CustomerUuid,
                Username = result.Customer.Username,
                Message = "ログインに成功しました。"
            };

            return Ok(response);
        }
        catch (AuthenticationException ex)
        {
            // 通常の認証失敗（パスワード間違いなど）の場合 (HTTP 401 Unauthorized)
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// 認証Cookieを設定する。
    /// </summary>
    /// <param name="accessToken">JWTアクセストークン</param>
    private void AppendAuthCookie(string accessToken)
    {
        Response.Cookies.Append(
        AuthCookieName,
        accessToken,
        new CookieOptions
        {
            HttpOnly = true,
            Secure = false,          // 開発環境ではHTTPのためfalse
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    /// ログアウトする
    /// </summary>
    /// <returns>成功メッセージ(200 OK)</returns>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(AuthCookieName);
        return Ok(new
        {
            Message = "ログアウトしました。"
        });
    }
}
