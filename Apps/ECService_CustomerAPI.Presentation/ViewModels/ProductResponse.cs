namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 商品一覧の1件分を表すレスポンス
/// </summary>
public class ProductResponse
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
    /// 商品画像URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
}