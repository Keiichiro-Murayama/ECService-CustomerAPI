using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 注文明細を取得するUsecaseのインターフェイス
/// </summary>
public interface IGetOrderDetailsUsecase
{
    /// <summary>
    /// 注文UUIDに紐づく注文明細を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>注文明細一覧</returns>
    Task<List<OrderDetail>> ExecuteAsync(string orderUuid);
}