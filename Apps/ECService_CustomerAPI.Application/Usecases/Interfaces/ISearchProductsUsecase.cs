using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 商品検索ユースケースのインターフェース
/// </summary>
public interface ISearchProductsUsecase
{
    /// <summary>
    /// カテゴリUUIDを条件に商品を検索する
    /// </summary>
    /// <param name="categoryUuid">
    /// カテゴリUUID。未指定の場合は全商品を取得する。
    /// </param>
    /// <returns>商品一覧</returns>
    Task<List<Product>> ExecuteAsync(
        string? categoryUuid);
}