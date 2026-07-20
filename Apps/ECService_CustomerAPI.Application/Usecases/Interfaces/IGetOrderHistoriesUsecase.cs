using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 購入履歴を取得するUsecaseのインターフェイス
/// </summary>
public interface IGetOrderHistoriesUsecase
{
    /// <summary>
    /// 顧客UUIDに紐づく購入履歴を取得する
    /// </summary>
    /// <param name="customerUuid">顧客UUID</param>
    /// <returns>購入履歴一覧</returns>
    Task<List<Orders>> ExecuteAsync(string customerUuid);
}