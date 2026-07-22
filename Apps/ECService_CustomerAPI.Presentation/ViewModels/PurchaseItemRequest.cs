namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 購入商品1件分のリクエスト
/// </summary>
public class PurchaseItemRequest
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; set; } = string.Empty;

    /// <summary>
    /// 購入数量
    /// </summary>
    public int Quantity { get; set; }
}