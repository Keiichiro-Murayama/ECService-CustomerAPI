namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 商品購入リクエスト
/// </summary>
public class PurchaseRequest
{
    /// <summary>
    /// 支払い方法ID
    /// </summary>
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// 購入商品一覧
    /// </summary>
    public List<PurchaseItemRequest> Items { get; set; } = new();
}