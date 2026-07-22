using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Adapters;

/// <summary>
/// 支払い方法ドメインを支払い方法レスポンスへ変換するAdapter
/// </summary>
public class GetPaymentMethodsViewModelAdapter
{
    /// <summary>
    /// 支払い方法一覧をレスポンス一覧へ変換する
    /// </summary>
    /// <param name="paymentMethods">
    /// 支払い方法一覧
    /// </param>
    /// <returns>支払い方法レスポンス一覧</returns>
    public Task<List<PaymentMethodResponse>> ConvertAsync(
        List<PaymentMethod> paymentMethods)
    {
        var responses = paymentMethods
            .Select(paymentMethod =>
                new PaymentMethodResponse
                {
                    PaymentMethodId =
                        paymentMethod.Id.ToString(),

                    PaymentMethodName =
                        paymentMethod.Name
                })
            .ToList();

        return Task.FromResult(responses);
    }
}