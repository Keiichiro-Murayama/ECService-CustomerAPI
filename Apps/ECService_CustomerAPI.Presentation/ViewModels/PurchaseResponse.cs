namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 商品購入レスポンス
/// </summary>
public class PurchaseResponse
{
    /// <summary>
    /// 登録された注文UUID
    /// </summary>
    public string OrderUuid { get; set; } = string.Empty;

    /// <summary>
    /// 処理結果メッセージ
    /// </summary>
    public string Message { get; set; } = string.Empty;
}