using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 商品カテゴリ一覧取得ユースケース
/// </summary>
public class GetCategoriesUsecase
    : IGetCategoriesUsecase
{
    private readonly IProductCategoryRepository
        _productCategoryRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productCategoryRepository">
    /// 商品カテゴリリポジトリ
    /// </param>
    public GetCategoriesUsecase(
        IProductCategoryRepository productCategoryRepository)
    {
        _productCategoryRepository =
            productCategoryRepository;
    }

    /// <summary>
    /// 商品カテゴリを全件取得する
    /// </summary>
    /// <returns>商品カテゴリ一覧</returns>
    public async Task<List<ProductCategory>> ExecuteAsync()
    {
        return await _productCategoryRepository
            .SelectAllAsync();
    }
}