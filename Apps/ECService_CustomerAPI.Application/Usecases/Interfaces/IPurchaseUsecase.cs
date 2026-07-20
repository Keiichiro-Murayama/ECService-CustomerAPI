using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 商品購入処理を行うユースケースのインターフェイス
/// </summary>
public interface IPurchaseUsecase
{
    /// <summary>
    /// 商品の購入を確定する
    /// </summary>
    /// <param name="customerUuid">
    /// JWTから取得した顧客UUID
    /// </param>
    /// <param name="paymentMethodId">
    /// 支払い方法ID
    /// </param>
    /// <param name="items">
    /// 購入商品のUUIDと数量
    /// </param>
    /// <returns>
    /// 登録した注文の注文UUID
    /// </returns>
    Task<string> ExecuteAsync(
        string customerUuid,
        int paymentMethodId,
        List<(string ProductUuid, int Quantity)> items);
}