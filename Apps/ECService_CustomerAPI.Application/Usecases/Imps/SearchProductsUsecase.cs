using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 商品検索ユースケース
/// </summary>
public class SearchProductsUsecase
    : ISearchProductsUsecase
{
    private readonly IProductRepository
        _productRepository;

    private readonly IProductCategoryRepository
        _productCategoryRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productRepository">
    /// 商品リポジトリ
    /// </param>
    /// <param name="productCategoryRepository">
    /// 商品カテゴリリポジトリ
    /// </param>
    public SearchProductsUsecase(
        IProductRepository productRepository,
        IProductCategoryRepository productCategoryRepository)
    {
        _productRepository = productRepository;
        _productCategoryRepository =
            productCategoryRepository;
    }

    /// <summary>
    /// カテゴリUUIDを条件に商品を検索する
    /// </summary>
    /// <param name="categoryUuid">
    /// カテゴリUUID。未指定の場合は全商品を取得する。
    /// </param>
    /// <returns>商品一覧</returns>
    public async Task<List<Product>> ExecuteAsync(
        string? categoryUuid)
    {
        var normalizedCategoryUuid =
            categoryUuid?.Trim();

        /*
         * カテゴリ未指定の場合は全商品を取得する
         */
        if (string.IsNullOrWhiteSpace(
            normalizedCategoryUuid))
        {
            return await _productRepository
                .SelectAllAsync();
        }

        /*
         * カテゴリUUIDの形式を検証する
         */
        if (!Guid.TryParse(
            normalizedCategoryUuid,
            out var parsedCategoryUuid))
        {
            throw new DomainException(
                "指定されたカテゴリID（UUID）が存在しません。",
                nameof(categoryUuid));
        }

        /*
         * 指定されたカテゴリが登録されているか確認する
         */
        var categories = await _productCategoryRepository
            .SelectAllAsync();

        var existsCategory = categories.Any(category =>
            Guid.TryParse(
                category.CategoryUuid,
                out var registeredCategoryUuid) &&
            registeredCategoryUuid ==
                parsedCategoryUuid);

        if (!existsCategory)
        {
            throw new DomainException(
                "指定されたカテゴリID（UUID）が存在しません。",
                nameof(categoryUuid));
        }

        /*
         * 指定カテゴリの商品を取得する
         */
        return await _productRepository
            .SelectByCategoryAsync(
                parsedCategoryUuid.ToString());
    }
}