using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Domain.Repositories;

/// <summary>
/// 商品カテゴリに関するデータアクセスのインターフェース
/// </summary>
public interface IProductCategoryRepository
{
    /// <summary>
    /// 商品カテゴリを全件取得する
    /// </summary>
    /// <returns>商品カテゴリ一覧</returns>
    Task<List<ProductCategory>> SelectAllAsync();
}