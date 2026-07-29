using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 商品カテゴリ一覧取得ユースケースのインターフェース
/// </summary>
public interface IGetCategoriesUsecase
{
    /// <summary>
    /// 商品カテゴリを全件取得する
    /// </summary>
    /// <returns>商品カテゴリ一覧</returns>
    Task<List<ProductCategory>> ExecuteAsync();
}