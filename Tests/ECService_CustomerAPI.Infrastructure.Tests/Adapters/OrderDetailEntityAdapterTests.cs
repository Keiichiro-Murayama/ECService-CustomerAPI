using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Infrastructure.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Adapters;

[TestClass]
public class OrderDetailEntityAdapterTests
{
    private OrderDetailEntityAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new OrderDetailEntityAdapter();
    }

    /// <summary>
    /// ConvertAsync：OrderDetailからOrderDetailEntityへ正しく変換される
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_ReturnsOrderDetailEntity_WhenOrderDetailIsValid()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var domain = OrderDetail.Restore(productUuid, 3);

        // Act
        var entity = await _adapter.ConvertAsync(domain);

        // Assert
        Assert.AreEqual(domain.Count, entity.Count);
        Assert.AreEqual(0, entity.ProductId);
    }

    /// <summary>
    /// ConvertWithProductIdAsync：ProductIdを指定して正しく変換される
    /// </summary>
    [TestMethod]
    public async Task ConvertWithProductIdAsync_ReturnsOrderDetailEntity_WhenProductIdIsSpecified()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var domain = OrderDetail.Restore(productUuid, 5);

        const int productId = 10;

        // Act
        var entity = await _adapter.ConvertWithProductIdAsync(domain, productId);

        // Assert
        Assert.AreEqual(productId, entity.ProductId);
        Assert.AreEqual(domain.Count, entity.Count);
    }

    /// <summary>
    /// RestoreAsync：OrderDetailEntityからOrderDetailへ正しく復元される
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ReturnsOrderDetail_WhenEntityIsValid()
    {
        // Arrange
        var productUuid = Guid.NewGuid();

        var entity = new OrderDetailEntity
        {
            ProductId = 1,
            Count = 2,
            Product = new ProductEntity
            {
                ProductUuid = productUuid
            }
        };

        // Act
        var domain = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual(productUuid.ToString(), domain.ProductUuid);
        Assert.AreEqual(entity.Count, domain.Count);
    }
}