using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 商品詳細ドメインを商品詳細レスポンスへ変換するAdapter
/// </summary>
public class GetProductDetailViewModelAdapter
{
    /// <summary>
    /// 商品詳細をレスポンスへ変換する
    /// </summary>
    /// <param name="productDetail">
    /// 商品詳細ドメインオブジェクト
    /// </param>
    /// <returns>商品詳細レスポンス</returns>
    public Task<ProductDetailResponse> ConvertAsync(
        ProductDetail productDetail)
    {
        var response = new ProductDetailResponse
        {
            ProductUuid =
                productDetail.ProductUuid,

            ProductName =
                productDetail.Name,

            Price =
                productDetail.Price,

            ImageUrl =
                productDetail.ImageUrl,

            Stock =
                productDetail.Stock,

            CategoryUuid =
                productDetail.CategoryUuid
        };

        return Task.FromResult(response);
    }
}