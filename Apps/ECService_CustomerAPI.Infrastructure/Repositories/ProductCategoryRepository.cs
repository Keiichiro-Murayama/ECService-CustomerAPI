using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService_CustomerAPI.Infrastructure.Repositories;

/// <summary>
/// 商品カテゴリに関するデータアクセスを行うリポジトリ
/// </summary>
public class ProductCategoryRepository
    : IProductCategoryRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context">DBコンテキスト</param>
    public ProductCategoryRepository(
        AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 商品カテゴリを全件取得する
    /// </summary>
    /// <returns>商品カテゴリ一覧</returns>
    public async Task<List<ProductCategory>> SelectAllAsync()
    {
        var categoryEntities = await _context.ProductCategories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .ToListAsync();

        return categoryEntities
            .Select(category => ProductCategory.Restore(
                category.CategoryUuid.ToString(),
                category.Name))
            .ToList();
    }
}