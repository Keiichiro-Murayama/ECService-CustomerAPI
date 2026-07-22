using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

[TestClass]
public class GetPaymentMethodsControllerTests
{
    private Mock<IGetPaymentMethodsUsecase> _usecaseMock = null!;
    private Mock<ILogger<GetPaymentMethodsController>> _loggerMock = null!;

    private GetPaymentMethodsViewModelAdapter _adapter = null!;
    private GetPaymentMethodsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _usecaseMock = new Mock<IGetPaymentMethodsUsecase>();
        _loggerMock = new Mock<ILogger<GetPaymentMethodsController>>();

        // Adapterは本物を使用
        _adapter = new GetPaymentMethodsViewModelAdapter();

        _controller = new GetPaymentMethodsController(
            _usecaseMock.Object,
            _adapter,
            _loggerMock.Object);
    }

    /// <summary>
    /// 支払い方法一覧を正常に取得できること
    /// </summary>
    [TestMethod]
    public async Task GetPaymentMethodsAsync_ReturnOk()
    {
        // Arrange
        var paymentMethods = new List<PaymentMethod>
        {
            PaymentMethod.Restore(1, "クレジットカード"),
            PaymentMethod.Restore(2, "銀行振込")
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(paymentMethods);

        // Act
        var result = await _controller.GetPaymentMethodsAsync();

        // Assert
        var okResult = result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

        var responses =
            okResult.Value as List<PaymentMethodResponse>;

        Assert.IsNotNull(responses);
        Assert.HasCount(2, responses);

        Assert.AreEqual("1", responses[0].PaymentMethodId);
        Assert.AreEqual("クレジットカード", responses[0].PaymentMethodName);

        Assert.AreEqual("2", responses[1].PaymentMethodId);
        Assert.AreEqual("銀行振込", responses[1].PaymentMethodName);

        _usecaseMock.Verify(x => x.ExecuteAsync(), Times.Once);
        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 支払い方法が0件でも正常終了すること
    /// </summary>
    [TestMethod]
    public async Task GetPaymentMethodsAsync_ReturnEmptyList()
    {
        // Arrange
        _usecaseMock
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(new List<PaymentMethod>());

        // Act
        var result = await _controller.GetPaymentMethodsAsync();

        // Assert
        var okResult = result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

        var responses =
            okResult.Value as List<PaymentMethodResponse>;

        Assert.IsNotNull(responses);
        Assert.IsEmpty(responses);

        _usecaseMock.Verify(x => x.ExecuteAsync(), Times.Once);
        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Usecaseで例外が発生した場合は500を返却すること
    /// </summary>
    [TestMethod]
    public async Task GetPaymentMethodsAsync_ReturnInternalServerError()
    {
        // Arrange
        _usecaseMock
            .Setup(x => x.ExecuteAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.GetPaymentMethodsAsync();

        // Assert
        var objectResult = result.Result as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode);

        _usecaseMock.Verify(x => x.ExecuteAsync(), Times.Once);
        _usecaseMock.VerifyNoOtherCalls();
    }
}