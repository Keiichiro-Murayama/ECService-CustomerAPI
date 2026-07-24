using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Domain.Tests.Models;

[TestClass]
public class OrderDetailTests
{
    /// <summary>
    /// Create：正常に生成できる
    /// </summary>
    [TestMethod]
    public void Create_ReturnsOrderDetail_WhenInputIsValid()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        // Act
        var result = OrderDetail.Create(productUuid, 1);

        // Assert
        Assert.AreEqual(productUuid, result.ProductUuid);
        Assert.AreEqual(1, result.Count);
    }

    /// <summary>
    /// Create：数量が複数でも生成できる
    /// </summary>
    [TestMethod]
    public void Create_ReturnsOrderDetail_WhenCountIsFive()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        // Act
        var result = OrderDetail.Create(productUuid, 5);

        // Assert
        Assert.AreEqual(productUuid, result.ProductUuid);
        Assert.AreEqual(5, result.Count);
    }

    /// <summary>
    /// Create：UUIDが空文字の場合
    /// </summary>
    [TestMethod]
    public void Create_ThrowsDomainException_WhenUuidIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            OrderDetail.Create("", 1));
    }

    /// <summary>
    /// Create：UUIDが空白の場合
    /// </summary>
    [TestMethod]
    public void Create_ThrowsDomainException_WhenUuidIsWhiteSpace()
    {
        Assert.Throws<DomainException>(() =>
            OrderDetail.Create(" ", 1));
    }

    /// <summary>
    /// Create：UUID形式が不正
    /// </summary>
    [TestMethod]
    public void Create_ThrowsDomainException_WhenUuidIsInvalid()
    {
        Assert.Throws<DomainException>(() =>
            OrderDetail.Create("abc", 1));
    }

    /// <summary>
    /// Create：数量が0
    /// </summary>
    [TestMethod]
    public void Create_ThrowsDomainException_WhenCountIsZero()
    {
        var productUuid = Guid.NewGuid().ToString();

        Assert.Throws<DomainException>(() =>
            OrderDetail.Create(productUuid, 0));
    }

    /// <summary>
    /// Create：数量が負数
    /// </summary>
    [TestMethod]
    public void Create_ThrowsDomainException_WhenCountIsNegative()
    {
        var productUuid = Guid.NewGuid().ToString();

        Assert.Throws<DomainException>(() =>
            OrderDetail.Create(productUuid, -1));
    }

    /// <summary>
    /// Restore：正常に復元できる
    /// </summary>
    [TestMethod]
    public void Restore_ReturnsOrderDetail_WhenInputIsValid()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        // Act
        var result = OrderDetail.Restore(productUuid, 2);

        // Assert
        Assert.AreEqual(productUuid, result.ProductUuid);
        Assert.AreEqual(2, result.Count);
    }

    /// <summary>
    /// Restore：UUIDが空文字
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenUuidIsEmpty()
    {
        Assert.Throws<DomainException>(() =>
            OrderDetail.Restore("", 2));
    }

    /// <summary>
    /// Restore：UUID形式が不正
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenUuidIsInvalid()
    {
        Assert.Throws<DomainException>(() =>
            OrderDetail.Restore("abc", 2));
    }

    /// <summary>
    /// Restore：数量が0
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenCountIsZero()
    {
        var productUuid = Guid.NewGuid().ToString();

        Assert.Throws<DomainException>(() =>
            OrderDetail.Restore(productUuid, 0));
    }

    /// <summary>
    /// Restore：数量が負数
    /// </summary>
    [TestMethod]
    public void Restore_ThrowsDomainException_WhenCountIsNegative()
    {
        var productUuid = Guid.NewGuid().ToString();

        Assert.Throws<DomainException>(() =>
            OrderDetail.Restore(productUuid, -5));
    }
}