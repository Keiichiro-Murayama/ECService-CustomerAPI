using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 購入履歴取得Usecaseの実装
/// </summary>
public class GetOrderHistoriesUsecase : IGetOrderHistoriesUsecase
{
    private readonly IOrdersRepository _ordersRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="ordersRepository">注文Repository</param>
    public GetOrderHistoriesUsecase(
        IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    /// <summary>
    /// 顧客UUIDに紐づく購入履歴を取得する
    /// </summary>
    /// <param name="customerUuid">顧客UUID</param>
    /// <returns>購入履歴一覧</returns>
    public async Task<List<Orders>> ExecuteAsync(
        string customerUuid)
    {
        var orders =
            await _ordersRepository.SelectByCustomerUuidAsync(
                customerUuid);

        return orders
            .OrderByDescending(order => order.OrderDate)
            .ToList();
    }
}