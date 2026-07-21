using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 顧客アカウント情報を取得するUsecaseのインターフェイス
/// </summary>
public interface IGetCustomerInfoUsecase
{
    /// <summary>
    /// 顧客UUIDに紐づく顧客アカウント情報を取得する
    /// </summary>
    /// <param name="customerUuid">顧客UUID</param>
    /// <returns>顧客アカウント情報。存在しない場合はnull</returns>
    Task<Customer?> ExecuteAsync(string customerUuid);
}