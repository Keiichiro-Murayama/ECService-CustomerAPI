using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Presentation.Tests.Adapters;

[TestClass]
public class GetCustomerInfoViewModelAdapterTests
{
    private GetCustomerInfoViewModelAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new GetCustomerInfoViewModelAdapter();
    }

    /// <summary>
    /// 正常系：CustomerをCustomerInfoResponseへ正しく変換できる
    /// </summary>
    [TestMethod]
    public void ConvertAsync_ReturnsCustomerInfoResponse()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();

        var customer = Customer.Restore(
            customerUuid,
            "山田太郎",
            "ヤマダタロウ",
            "東京都新宿区",
            "〇〇ビル101",
            "03-1234-5678",
            "test@example.com",
            "testuser",
            "passwordHash");

        // Act
        var result = _adapter.ConvertAsync(customer);

        // Assert
        Assert.IsNotNull(result);

        Assert.AreEqual(customer.CustomerUuid, result.CustomerUuid);
        Assert.AreEqual(customer.Name, result.Name);
        Assert.AreEqual(customer.NameKana, result.NameKana);
        Assert.AreEqual(customer.Address1, result.Address1);
        Assert.AreEqual(customer.Address2, result.Address2);
        Assert.AreEqual(customer.PhoneNumber, result.PhoneNumber);
        Assert.AreEqual(customer.MailAddress, result.MailAddress);
        Assert.AreEqual(customer.Username, result.Username);
    }
}