using System;

namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 購入履歴取得APIのレスポンス
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// 注文ID
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// 注文UUID
    /// </summary>
    public string OrderUuid { get; set; } = string.Empty;

    /// <summary>
    /// 注文日時
    /// </summary>
    public DateTimeOffset OrderDate { get; set; }

    /// <summary>
    /// 注文合計金額
    /// </summary>
    public int AmountTotal { get; set; }
}