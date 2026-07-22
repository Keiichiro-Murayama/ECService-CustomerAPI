using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 注文明細ドメインをレスポンスへ変換するAdapter
/// </summary>
public class GetOrderDetailsViewModelAdapter
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productRepository">商品Repository</param>
    public GetOrderDetailsViewModelAdapter(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// 注文明細一覧をレスポンス一覧へ変換する
    /// </summary>
    /// <param name="orderDetails">注文明細一覧</param>
    /// <returns>注文明細レスポンス一覧</returns>
    public async Task<List<OrderDetailResponse>> ConvertAsync(
        List<OrderDetail> orderDetails)
    {
        var responses = new List<OrderDetailResponse>();

        foreach (var orderDetail in orderDetails)
        {
            var productName =
                await _productRepository.SelectNameByProductUuidAsync(
                    orderDetail.ProductUuid);

            var price =
                await _productRepository.SelectPriceByProductUuidAsync(
                    orderDetail.ProductUuid);

            responses.Add(new OrderDetailResponse
            {
                ProductUuid = orderDetail.ProductUuid,
                ProductName = productName,
                Price = price,
                Quantity = orderDetail.Count
            });
        }

        return responses;
    }
}