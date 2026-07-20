namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 注文明細取得APIのレスポンス
/// </summary>
public class OrderDetailResponse
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; set; } = string.Empty;

    /// <summary>
    /// 商品名
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 商品価格
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// 購入個数
    /// </summary>
    public int Quantity { get; set; }
}