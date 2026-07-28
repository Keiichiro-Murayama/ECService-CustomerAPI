namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 商品詳細取得APIのレスポンス
/// </summary>
public class ProductDetailResponse
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; set; }
        = string.Empty;

    /// <summary>
    /// 商品名
    /// </summary>
    public string ProductName { get; set; }
        = string.Empty;

    /// <summary>
    /// 商品価格
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// 商品画像URL
    /// </summary>
    public string ImageUrl { get; set; }
        = string.Empty;

    /// <summary>
    /// 商品在庫数
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// カテゴリUUID
    /// </summary>
    public string CategoryUuid { get; set; }
        = string.Empty;
}