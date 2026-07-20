namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 商品購入リクエスト
/// </summary>
public class PurchaseRequest
{
    /// <summary>
    /// 支払い方法ID
    /// JSONでは「"1"」のような文字列として受け取る
    /// </summary>
    public string PaymentMethodId { get; set; } = string.Empty;

    /// <summary>
    /// 購入商品一覧
    /// </summary>
    public List<PurchaseItemRequest> Items { get; set; } = new();
}