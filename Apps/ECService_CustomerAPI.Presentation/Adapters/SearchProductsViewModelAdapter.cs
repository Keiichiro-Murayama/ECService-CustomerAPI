using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 商品ドメインを商品検索レスポンスへ変換するAdapter
/// </summary>
public class SearchProductsViewModelAdapter
{
    /// <summary>
    /// 商品一覧をレスポンス一覧へ変換する
    /// </summary>
    /// <param name="products">商品ドメインの一覧</param>
    /// <returns>商品レスポンス一覧</returns>
    public Task<List<ProductResponse>> ConvertAsync(
        List<Product> products)
    {
        var responses = products
            .Select(product => new ProductResponse
            {
                ProductUuid = product.ProductUuid,
                ProductName = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            })
            .ToList();

        return Task.FromResult(responses);
    }
}