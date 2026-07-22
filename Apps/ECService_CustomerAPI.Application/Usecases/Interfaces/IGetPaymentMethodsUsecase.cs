using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Application.Usecases.Interfaces;

/// <summary>
/// 支払い方法一覧を取得するUsecaseのインターフェイス
/// </summary>
public interface IGetPaymentMethodsUsecase
{
    /// <summary>
    /// すべての支払い方法を取得する
    /// </summary>
    /// <returns>支払い方法一覧</returns>
    Task<List<PaymentMethod>> ExecuteAsync();
}