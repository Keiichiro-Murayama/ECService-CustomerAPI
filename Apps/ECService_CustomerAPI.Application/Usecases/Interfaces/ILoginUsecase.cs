using ECService_CustomerAPI.Domain.Models;
namespace ECService_CustomerAPI.Application.Usecases.Interfaces;
/// <summary>
/// ログイン(認証)を行うユースケースのインターフェイス(UC-02)
/// </summary>
public interface ILoginUsecase
{
    /// <summary>
    /// メールアドレスとパスワードで認証し、成功すれば JWT を発行する
    /// </summary>
    /// <param name="input">ログインの入力情報(メールアドレス・パスワード)</param>
    /// <returns>発行されたJWTを含むログイン結果</returns>
    Task<(string AccessToken, Customer Customer)> ExecuteAsync((string MailAddress, string Password) input);
}
