using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 注文ドメインを購入履歴レスポンスへ変換するAdapter
/// </summary>
public class GetOrderHistoriesViewModelAdapter
{
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
            OrderDate = order.OrderDate.GetValueOrDefault(),
            AmountTotal = order.AmountTotal
        }).ToList();
    }
}