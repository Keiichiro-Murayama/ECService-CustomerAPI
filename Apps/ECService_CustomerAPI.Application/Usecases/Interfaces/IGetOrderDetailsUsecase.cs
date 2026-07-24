using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 注文明細を取得するUsecaseのインターフェイス
/// </summary>
public interface IGetOrderDetailsUsecase
{
    /// <summary>
    /// 注文UUIDと顧客UUIDに紐づく注文明細を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <param name="customerUuid">ログイン中の顧客UUID</param>
    /// <returns>注文明細一覧</returns>
    //石原:変更 他顧客の注文明細取得を防ぐため、顧客UUIDを引数に追加
    Task<List<OrderDetail>> ExecuteAsync(
        string orderUuid,
        string customerUuid);
}