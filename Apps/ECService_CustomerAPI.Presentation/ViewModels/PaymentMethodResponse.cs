namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 支払い方法一覧取得APIのレスポンス
/// </summary>
public class PaymentMethodResponse
{
    /// <summary>
    /// 支払い方法ID
    /// </summary>
    public string PaymentMethodId { get; set; } = string.Empty;

    /// <summary>
    /// 支払い方法名
    /// </summary>
    public string PaymentMethodName { get; set; } = string.Empty;
}