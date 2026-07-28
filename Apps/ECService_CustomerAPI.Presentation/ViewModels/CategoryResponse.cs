namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 商品カテゴリ一覧の1件分を表すレスポンス
/// </summary>
public class CategoryResponse
{
    /// <summary>
    /// カテゴリUUID
    /// </summary>
    public string CategoryUuid { get; set; } = string.Empty;

    /// <summary>
    /// カテゴリ名
    /// </summary>
    public string Name { get; set; } = string.Empty;
}