using ECService_CustomerAPI.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Domain.Tests.Models;

[TestClass]
public class PaymentMethodTests
{
    /// <summary>
    /// Restore：正常な値でPaymentMethodを復元できる
    /// </summary>
    [TestMethod]
    public void Restore_ReturnsPaymentMethod_WhenArgumentsAreValid()
    {
        // Arrange
        const int id = 1;
        const string name = "現金";

        // Act
        var paymentMethod = PaymentMethod.Restore(id, name);

        // Assert
        Assert.AreEqual(id, paymentMethod.Id);
        Assert.AreEqual(name, paymentMethod.Name);
    }
}