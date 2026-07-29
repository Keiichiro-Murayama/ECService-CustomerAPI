using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 商品カテゴリドメインをカテゴリレスポンスへ変換するAdapter
/// </summary>
public class GetCategoriesViewModelAdapter
{
    /// <summary>
    /// 商品カテゴリ一覧をレスポンス一覧へ変換する
    /// </summary>
    /// <param name="categories">
    /// 商品カテゴリドメインの一覧
    /// </param>
    /// <returns>カテゴリレスポンス一覧</returns>
    public Task<List<CategoryResponse>> ConvertAsync(
        List<ProductCategory> categories)
    {
        var responses = categories
            .Select(category => new CategoryResponse
            {
                CategoryUuid = category.CategoryUuid,
                Name = category.Name
            })
            .ToList();

        return Task.FromResult(responses);
    }
}