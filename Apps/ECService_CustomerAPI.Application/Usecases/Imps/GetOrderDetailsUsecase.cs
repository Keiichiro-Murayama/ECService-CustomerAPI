using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 注文明細取得Usecaseの実装
/// </summary>
public class GetOrderDetailsUsecase : IGetOrderDetailsUsecase
{
    private readonly IOrdersRepository _ordersRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ordersRepository">注文Repository</param>
    public GetOrderDetailsUsecase(
        IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    /// <summary>
    /// 注文UUIDに紐づく注文明細を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>注文明細一覧</returns>
    public async Task<List<OrderDetail>> ExecuteAsync(
        string orderUuid)
    {
        return await _ordersRepository
            .SelectOrderDetailsByOrderUuidAsync(orderUuid);
    }
}