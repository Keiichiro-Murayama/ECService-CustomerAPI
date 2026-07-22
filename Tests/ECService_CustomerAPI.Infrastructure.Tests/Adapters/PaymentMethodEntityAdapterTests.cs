using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Infrastructure.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Adapters;

[TestClass]
public class PaymentMethodEntityAdapterTests
{
    private PaymentMethodEntityAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new PaymentMethodEntityAdapter();
    }

    /// <summary>
    /// ConvertAsync：PaymentMethodからPaymentMethodEntityへ正しく変換される
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_ReturnsPaymentMethodEntity_WhenPaymentMethodIsValid()
    {
        // Arrange
        var model = PaymentMethod.Restore(
            1,
            "現金"
        );

        // Act
        var entity = await _adapter.ConvertAsync(model);

        // Assert
        Assert.AreEqual(model.Id, entity.Id);
        Assert.AreEqual(model.Name, entity.Name);
    }

    /// <summary>
    /// RestoreAsync：PaymentMethodEntityからPaymentMethodへ正しく復元される
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ReturnsPaymentMethod_WhenEntityIsValid()
    {
        // Arrange
        var entity = new PaymentMethodEntity
        {
            Id = 1,
            Name = "現金"
        };

        // Act
        var model = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual(entity.Id, model.Id);
        Assert.AreEqual(entity.Name, model.Name);
    }
}