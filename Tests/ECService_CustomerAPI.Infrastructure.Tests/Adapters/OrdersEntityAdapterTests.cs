using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Infrastructure.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Adapters;

[TestClass]
public class OrdersEntityAdapterTests
{
    private OrdersEntityAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new OrdersEntityAdapter();
    }

    /// <summary>
    /// ConvertAsync：サポート対象外メソッド
    /// </summary>
    [TestMethod]
    public async Task ConvertAsync_ThrowsInternalException()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();

        var order = Orders.Restore(
            1,
            Guid.NewGuid().ToString(),
            DateTime.UtcNow,
            1000,
            customerUuid,
            1,
            1,
            new List<OrderDetail>());

        // Act & Assert
        await Assert.ThrowsAsync<InternalException>(
            () => _adapter.ConvertAsync(order));
    }

    /// <summary>
    /// ConvertWithCustomerIdOrderDetailsAsync：正常変換
    /// </summary>
    [TestMethod]
    public async Task ConvertWithCustomerIdOrderDetailsAsync_ReturnsOrdersEntity()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();

        var details = new List<OrderDetail>
        {
            OrderDetail.Restore(Guid.NewGuid().ToString(), 2)
        };

        var order = Orders.Restore(
            1,
            Guid.NewGuid().ToString(),
            DateTime.UtcNow,
            5000,
            customerUuid,
            2,
            3,
            details);

        var entities = new List<OrderDetailEntity>
        {
            new()
            {
                ProductId = 10,
                Count = 2
            }
        };

        // Act
        var result = await _adapter.ConvertWithCustomerIdOrderDetailsAsync(
            order,
            100,
            entities);

        // Assert
        Assert.AreEqual(Guid.Parse(order.OrderUuid), result.OrderUuid);
        Assert.AreEqual(order.OrderDate, result.OrderDate);
        Assert.AreEqual(order.AmountTotal, result.AmountTotal);
        Assert.AreEqual(100, result.CustomerId);
        Assert.AreEqual(order.OrderStatusId, result.OrderStatusId);
        Assert.AreEqual(order.PaymentMethodId, result.PaymentMethodId);
    }

    /// <summary>
    /// ConvertWithCustomerIdOrderDetailsAsync：注文明細が設定される
    /// </summary>
    [TestMethod]
    public async Task ConvertWithCustomerIdOrderDetailsAsync_SetsOrderDetails()
    {
        // Arrange
        var order = Orders.Restore(
            1,
            Guid.NewGuid().ToString(),
            DateTime.UtcNow,
            1000,
            Guid.NewGuid().ToString(),
            1,
            1,
            new List<OrderDetail>());

        var orderDetails = new List<OrderDetailEntity>
        {
            new()
            {
                ProductId = 1,
                Count = 3
            }
        };

        // Act
        var entity = await _adapter.ConvertWithCustomerIdOrderDetailsAsync(
            order,
            1,
            orderDetails);

        // Assert
        Assert.AreSame(orderDetails, entity.OrdersDetails);
    }

    /// <summary>
    /// RestoreAsync：正常復元
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ReturnsOrders()
    {
        // Arrange
        var customerUuid = Guid.NewGuid();
        var productUuid = Guid.NewGuid();

        var entity = new OrdersEntity
        {
            Id = 1,
            OrderUuid = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            AmountTotal = 3000,
            PaymentMethodId = 2,

            Customer = new CustomerEntity
            {
                CustomerUuid = customerUuid
            },

            OrderStatus = new OrderStatusEntity
            {
                Id = 3
            },

            OrdersDetails = new List<OrderDetailEntity>
            {
                new()
                {
                    Count = 5,
                    Product = new ProductEntity
                    {
                        ProductUuid = productUuid
                    }
                }
            }
        };

        // Act
        var domain = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual(entity.Id.Value, domain.Id);
        Assert.AreEqual(entity.OrderUuid.ToString(), domain.OrderUuid);
        Assert.AreEqual(entity.OrderDate.Value, domain.OrderDate);
        Assert.AreEqual(entity.AmountTotal, domain.AmountTotal);
        Assert.AreEqual(customerUuid.ToString(), domain.CustomerUuid);
        Assert.AreEqual(entity.OrderStatus.Id, domain.OrderStatusId);
        Assert.AreEqual(entity.PaymentMethodId, domain.PaymentMethodId);

        Assert.HasCount(1, domain.OrderDetails);
        Assert.AreEqual(productUuid.ToString(), domain.OrderDetails[0].ProductUuid);
        Assert.AreEqual(5, domain.OrderDetails[0].Count);
    }

    /// <summary>
    /// RestoreAsync：OrderDateがnull
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ThrowsInternalException_WhenOrderDateIsNull()
    {
        // Arrange
        var entity = new OrdersEntity
        {
            Id = 1,
            OrderUuid = Guid.NewGuid(),
            OrderDate = null,
            AmountTotal = 1000,
            PaymentMethodId = 1,
            Customer = new CustomerEntity
            {
                CustomerUuid = Guid.NewGuid()
            },
            OrderStatus = new OrderStatusEntity
            {
                Id = 1
            },
            OrdersDetails = new List<OrderDetailEntity>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<InternalException>(
            () => _adapter.RestoreAsync(entity));
    }
}