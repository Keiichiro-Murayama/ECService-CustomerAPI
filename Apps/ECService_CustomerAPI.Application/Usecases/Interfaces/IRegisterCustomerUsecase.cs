using System.Threading.Tasks;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 顧客アカウント登録を行うUsecaseのインターフェイス
/// </summary>
public interface IRegisterCustomerUsecase
{
    /// <summary>
    /// 顧客情報を登録する
    /// </summary>
    /// <param name="input">顧客アカウント登録情報</param>
    /// <returns>登録した顧客のUUID</returns>
    Task<string> ExecuteAsync(
        (
            string Name,
            string NameKana,
            string Address1,
            string Address2,
            string PhoneNumber,
            string MailAddress,
            string Username,
            string Password
        ) input);
}