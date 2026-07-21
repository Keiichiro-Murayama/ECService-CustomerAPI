using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Application.Tests.Usecases;

[TestClass]
public class GetPaymentMethodsUsecaseTests
{
    private Mock<IPaymentMethodRepository> _repositoryMock = null!;
    private GetPaymentMethodsUsecase _usecase = null!;

    [TestInitialize]
    public void Setup()
    {
        _repositoryMock = new Mock<IPaymentMethodRepository>();
        _usecase = new GetPaymentMethodsUsecase(_repositoryMock.Object);
    }

    /// <summary>
    /// 支払い方法一覧が取得できること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnPaymentMethods()
    {
        // Arrange
        var paymentMethods = new List<PaymentMethod>
        {
            PaymentMethod.Restore(1, "クレジットカード"),
            PaymentMethod.Restore(2, "銀行振込")
        };
        _repositoryMock
            .Setup(x => x.SelectAllAsync())
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.HasCount(2,result);
        CollectionAssert.AreEqual(paymentMethods, result);

     _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
    }

    /// <summary>
    /// 支払い方法が存在しない場合は空リストを返すこと
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnEmptyList()
    {
        // Arrange
        var paymentMethods = new List<PaymentMethod>();

        _repositoryMock
            .Setup(x => x.SelectAllAsync())
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Repositoryで例外が発生した場合は例外が送出されること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ThrowException_WhenRepositoryThrows()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.SelectAllAsync())
            .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _usecase.ExecuteAsync());

        _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }
}