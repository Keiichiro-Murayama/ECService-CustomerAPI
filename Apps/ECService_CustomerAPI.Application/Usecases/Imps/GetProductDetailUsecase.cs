using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 商品詳細取得Usecase
/// </summary>
public class GetProductDetailUsecase
    : IGetProductDetailUsecase
{
    private readonly IProductRepository
        _productRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productRepository">
    /// 商品Repository
    /// </param>
    public GetProductDetailUsecase(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// 商品UUIDから商品詳細を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品詳細</returns>
    public async Task<ProductDetail> ExecuteAsync(
        string productUuid)
    {
        var normalizedProductUuid =
            productUuid?.Trim();

        /*
         * 商品UUIDの形式を検証する
         */
        if (string.IsNullOrWhiteSpace(
                normalizedProductUuid) ||
            !Guid.TryParse(
                normalizedProductUuid,
                out var parsedProductUuid))
        {
            throw new DomainException(
                "商品UUIDの形式が正しくありません。",
                nameof(productUuid));
        }

        /*
         * 商品詳細を取得する
         */
        var productDetail =
            await _productRepository.SelectByUuidAsync(
                parsedProductUuid.ToString());

        /*
         * 商品が存在しない、または削除済みの場合
         */
        if (productDetail == null)
        {
            throw new NotFoundException(
                "指定された商品が存在しません。");
        }

        return productDetail;
    }
}