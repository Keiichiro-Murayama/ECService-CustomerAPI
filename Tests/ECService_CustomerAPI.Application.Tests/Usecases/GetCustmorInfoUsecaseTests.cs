using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Application.Tests.Usecases;

[TestClass]
public class GetCustomerInfoUsecaseTests
{
    private Mock<ICustomerRepository> _customerRepositoryMock = null!;
    private GetCustomerInfoUsecase _usecase = null!;

    [TestInitialize]
    public void Initialize()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();

        _usecase = new GetCustomerInfoUsecase(
            _customerRepositoryMock.Object);
    }

    /// <summary>
    /// 正常系：顧客情報が存在する場合
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnsCustomer_WhenCustomerExists()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();

        var expected = Customer.Restore(
            customerUuid,
            "山田太郎",
            "ヤマダタロウ",
            "東京都新宿区",
            "〇〇ビル101",
            "03-1234-5678",
            "test@example.com",
            "testuser",
            "passwordHash");

        _customerRepositoryMock
            .Setup(x => x.FindByUuidAsync(customerUuid))
            .ReturnsAsync(expected);

        // Act
        var result = await _usecase.ExecuteAsync(customerUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(customerUuid, result.CustomerUuid);
        Assert.AreEqual("山田太郎", result.Name);
        Assert.AreEqual("ヤマダタロウ", result.NameKana);

        _customerRepositoryMock.Verify(
            x => x.FindByUuidAsync(customerUuid),
            Times.Once);

        _customerRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 正常系：顧客情報が存在しない場合
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();

        _customerRepositoryMock
            .Setup(x => x.FindByUuidAsync(customerUuid))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _usecase.ExecuteAsync(customerUuid);

        // Assert
        Assert.IsNull(result);

        _customerRepositoryMock.Verify(
            x => x.FindByUuidAsync(customerUuid),
            Times.Once);

        _customerRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 異常系：Repositoryが例外を送出する場合
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ThrowsException_WhenRepositoryThrowsException()
    {
        // Arrange
        var customerUuid = Guid.NewGuid().ToString();

        _customerRepositoryMock
            .Setup(x => x.FindByUuidAsync(customerUuid))
            .ThrowsAsync(new Exception("Repository Error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _usecase.ExecuteAsync(customerUuid));

        _customerRepositoryMock.Verify(
            x => x.FindByUuidAsync(customerUuid),
            Times.Once);

        _customerRepositoryMock.VerifyNoOtherCalls();
    }
}