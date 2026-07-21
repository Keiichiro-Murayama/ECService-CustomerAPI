using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Presentation.Tests.Adapters;

[TestClass]
public class GetPaymentMethodsViewModelAdapterTests
{
    private GetPaymentMethodsViewModelAdapter _adapter = null!;

    [TestInitialize]
    public void Setup()
    {
        _adapter = new GetPaymentMethodsViewModelAdapter();
    }

    /// <summary>
    /// 支払い方法一覧をレスポンス一覧へ変換できること
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_ReturnPaymentMethodResponses()
    {
        // Arrange
        var paymentMethods = new List<PaymentMethod>
        {
            PaymentMethod.Restore(1, "クレジットカード"),
            PaymentMethod.Restore(2, "銀行振込")
        };

        // Act
        var result = await _adapter.ConvertAsync(paymentMethods);

        // Assert
        Assert.HasCount(2,result);

        Assert.AreEqual("1", result[0].PaymentMethodId);
        Assert.AreEqual("クレジットカード", result[0].PaymentMethodName);

        Assert.AreEqual("2", result[1].PaymentMethodId);
        Assert.AreEqual("銀行振込", result[1].PaymentMethodName);
    }

    /// <summary>
    /// 支払い方法が0件の場合は空のレスポンス一覧を返すこと
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_ReturnEmptyResponse()
    {
        // Arrange
        var paymentMethods = new List<PaymentMethod>();

        // Act
        var result = await _adapter.ConvertAsync(paymentMethods);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }
}