using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Domain.Tests.Models;

[TestClass]
public class OrdersTests
{
    /// <summary>
    /// Create：正常な値でOrdersを生成できる
    /// </summary>
    [TestMethod]
    public void Create_ReturnsOrders_WhenArgumentsAreValid()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();
        var details = new List<OrderDetail>
        {
            OrderDetail.Create(Guid.NewGuid().ToString(), 2)
        };

        // Act
        var orders = Orders.Create(
            5000,
            customerUuid,
            1,
            details);

        // Assert
        Assert.AreEqual(5000, orders.AmountTotal);
        Assert.AreEqual(customerUuid, orders.CustomerUuid);
        Assert.AreEqual(1, orders.PaymentMethodId);
        Assert.AreSame(details, orders.OrderDetails);
    }

    /// <summary>
    /// Create：OrderUuidが自動採番される
    /// </summary>
    [TestMethod]
    public void Create_SetsOrderUuid()
    {
        // Arrange
        var details = new List<OrderDetail>();

        // Act
        var orders = Orders.Create(
            1000,
            Guid.NewGuid().ToString(),
            1,
            details);

        // Assert
        Assert.IsTrue(Guid.TryParse(orders.OrderUuid, out _));
    }

    /// <summary>
    /// Create：OrderStatusIdの初期値が1になる
    /// </summary>
    [TestMethod]
    public void Create_SetsInitialOrderStatusId()
    {
        // Arrange
        var details = new List<OrderDetail>();

        // Act
        var orders = Orders.Create(
            1000,
            Guid.NewGuid().ToString(),
            2,
            details);

        // Assert
        Assert.AreEqual(1, orders.OrderStatusId);
    }

    /// <summary>
    /// Restore：正常な値でOrdersを復元できる
    /// </summary>
    [TestMethod]
    public void Restore_ReturnsOrders_WhenArgumentsAreValid()
    {
        // Arrange
        var orderUuid = Guid.NewGuid().ToString();
        var customerUuid = Guid.NewGuid().ToString();
        var orderDate = DateTimeOffset.UtcNow;

        var details = new List<OrderDetail>
        {
            OrderDetail.Create(Guid.NewGuid().ToString(), 3)
        };

        // Act
        var orders = Orders.Restore(
            1,
            orderUuid,
            orderDate,
            5000,
            customerUuid,
            2,
            3,
            details);

        // Assert
        Assert.AreEqual(1, orders.Id);
        Assert.AreEqual(orderUuid, orders.OrderUuid);
        Assert.AreEqual(orderDate, orders.OrderDate);
        Assert.AreEqual(5000, orders.AmountTotal);
        Assert.AreEqual(customerUuid, orders.CustomerUuid);
        Assert.AreEqual(2, orders.OrderStatusId);
        Assert.AreEqual(3, orders.PaymentMethodId);
        Assert.AreSame(details, orders.OrderDetails);
    }

    /// <summary>
    /// Restore：OrderUuidが空文字の場合は例外
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenOrderUuidIsEmpty()
    {
        // Arrange
        var details = new List<OrderDetail>();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            Orders.Restore(
                1,
                "",
                DateTimeOffset.UtcNow,
                1000,
                Guid.NewGuid().ToString(),
                1,
                1,
                details));

        Assert.AreEqual("識別Idは必須です。", ex.Message);
    }

    /// <summary>
    /// Restore：OrderUuidがnullの場合は例外
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenOrderUuidIsNull()
    {
        // Arrange
        var details = new List<OrderDetail>();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            Orders.Restore(
                1,
                null!,
                DateTimeOffset.UtcNow,
                1000,
                Guid.NewGuid().ToString(),
                1,
                1,
                details));

        Assert.AreEqual("識別Idは必須です。", ex.Message);
    }

    /// <summary>
    /// Restore：OrderUuidがGUID形式でない場合は例外
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenOrderUuidIsInvalid()
    {
        // Arrange
        var details = new List<OrderDetail>();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() =>
            Orders.Restore(
                1,
                "abc",
                DateTimeOffset.UtcNow,
                1000,
                Guid.NewGuid().ToString(),
                1,
                1,
                details));

        Assert.AreEqual("識別Idの形式が不正です。", ex.Message);
    }
}