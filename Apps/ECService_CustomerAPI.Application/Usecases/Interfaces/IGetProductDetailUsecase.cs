using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 商品詳細取得Usecaseのインターフェース
/// </summary>
public interface IGetProductDetailUsecase
{
    /// <summary>
    /// 商品UUIDから商品詳細を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品詳細</returns>
    Task<ProductDetail> ExecuteAsync(
        string productUuid);
}