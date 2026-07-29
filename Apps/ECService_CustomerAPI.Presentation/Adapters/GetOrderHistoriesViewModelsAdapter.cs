using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 注文ドメインを購入履歴レスポンスへ変換するAdapter
/// </summary>
public class GetOrderHistoriesViewModelAdapter
{
    /// <summary>
    /// 日本時間のUTCオフセット
    /// </summary>
    private static readonly TimeSpan JapanOffset = TimeSpan.FromHours(9);

    /// <summary>
    /// 注文一覧を購入履歴レスポンス一覧へ変換する
    /// </summary>
    /// <param name="orders">注文一覧</param>
    /// <returns>購入履歴レスポンス一覧</returns>
    public List<OrderResponse> ConvertAsync(List<Orders> orders)
    {
        return orders.Select(order => new OrderResponse
        {
            OrderId = order.Id.GetValueOrDefault(),
            OrderUuid = order.OrderUuid,

            // DBから取得したUTC日時を日本時間へ変換する
            OrderDate = order.OrderDate
                .GetValueOrDefault()
                .ToOffset(JapanOffset),

            AmountTotal = order.AmountTotal
        }).ToList();
    }
}