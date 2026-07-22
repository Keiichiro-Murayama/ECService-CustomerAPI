using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;

namespace ECService_CustomerAPI.Application.Usecases.Imps;

/// <summary>
/// 支払い方法一覧取得Usecaseの実装
/// </summary>
public class GetPaymentMethodsUsecase : IGetPaymentMethodsUsecase
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="paymentMethodRepository">
    /// 支払い方法Repository
    /// </param>
    public GetPaymentMethodsUsecase(
        IPaymentMethodRepository paymentMethodRepository)
    {
        _paymentMethodRepository = paymentMethodRepository;
    }

    /// <summary>
    /// すべての支払い方法を取得する
    /// </summary>
    /// <returns>支払い方法一覧</returns>
    public async Task<List<PaymentMethod>> ExecuteAsync()
    {
        return await _paymentMethodRepository.SelectAllAsync();
    }
}