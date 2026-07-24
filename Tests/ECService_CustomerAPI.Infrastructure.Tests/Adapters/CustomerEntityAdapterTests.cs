using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Infrastructure.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Adapters;

[TestClass]
public class CustomerEntityAdapterTests
{
    private CustomerEntityAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new CustomerEntityAdapter();
    }

    /// <summary>
    /// ConvertAsync：全プロパティが正しく変換される
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_ReturnsCustomerEntity_WhenCustomerIsValid()
    {
        // Arrange
        var customer = Customer.Restore(
            Guid.NewGuid().ToString(),
            "山田太郎",
            "ヤマダタロウ",
            "東京都渋谷区1-11-11",
            "マンション渋谷101号室",
            "03-1111-2222",
            "taro@example.com",
            "taro123",
            "password123"
        );

        // Act
        var entity = await _adapter.ConvertAsync(customer);

        // Assert
        Assert.AreEqual(Guid.Parse(customer.CustomerUuid), entity.CustomerUuid);
        Assert.AreEqual(customer.Name, entity.Name);
        Assert.AreEqual(customer.NameKana, entity.NameKana);
        Assert.AreEqual(customer.Address1, entity.Address1);
        Assert.AreEqual(customer.Address2, entity.Address2);
        Assert.AreEqual(customer.PhoneNumber, entity.PhoneNumber);
        Assert.AreEqual(customer.MailAddress, entity.MailAddress);
        Assert.AreEqual(customer.Username, entity.Username);
        Assert.AreEqual(customer.PasswordHash, entity.Password);
    }

    /// <summary>
    /// ConvertAsync：CreatedAtがUTC現在時刻で設定される
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_SetsCreatedAtToUtcNow()
    {
        // Arrange
        var customer = Customer.Restore(
            Guid.NewGuid().ToString(),
            "山田太郎",
            "ヤマダタロウ",
            "東京都渋谷区1-11-11",
            "マンション渋谷101号室",
            "03-1111-2222",
            "taro@example.com",
            "taro123",
            "password123"
        );

        var before = DateTime.UtcNow;

        // Act
        var entity = await _adapter.ConvertAsync(customer);

        var after = DateTime.UtcNow;

        // Assert
        Assert.IsTrue(entity.CreatedAt >= before);
        Assert.IsTrue(entity.CreatedAt <= after);
    }

    /// <summary>
    /// RestoreAsync：全プロパティが正しく復元される
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ReturnsCustomer_WhenEntityIsValid()
    {
        // Arrange
        var entity = new CustomerEntity
        {
            CustomerUuid = Guid.NewGuid(),
            Name = "山田太郎",
            NameKana = "ヤマダタロウ",
            Address1 = "東京都渋谷区1-11-11",
            Address2 = "マンション渋谷101号室",
            PhoneNumber = "03-1111-2222",
            MailAddress = "taro@example.com",
            Username = "taro123",
            Password = "password123"
        };

        // Act
        var customer = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual(entity.CustomerUuid.ToString(), customer.CustomerUuid);
        Assert.AreEqual(entity.Name, customer.Name);
        Assert.AreEqual(entity.NameKana, customer.NameKana);
        Assert.AreEqual(entity.Address1, customer.Address1);
        Assert.AreEqual(entity.Address2, customer.Address2);
        Assert.AreEqual(entity.PhoneNumber, customer.PhoneNumber);
        Assert.AreEqual(entity.MailAddress, customer.MailAddress);
        Assert.AreEqual(entity.Username, customer.Username);
        Assert.AreEqual(entity.Password, customer.PasswordHash);
    }

}