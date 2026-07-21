using System.Security.Claims;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

[TestClass]
public class GetCustomerInfoControllerTests
{
    private Mock<IGetCustomerInfoUsecase> _usecaseMock = null!;
    private GetCustomerInfoViewModelAdapter _adapter = null!;
    private GetCustomerInfoController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock = new Mock<IGetCustomerInfoUsecase>();

        // Adapterは実インスタンスを使用
        _adapter = new GetCustomerInfoViewModelAdapter();

        _controller = new GetCustomerInfoController(
            _usecaseMock.Object,
            _adapter);
    }

    private void SetUser(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "Test");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }

    /// <summary>
    /// 正常系：NameIdentifierからUUIDを取得できる場合
    /// </summary>
    [TestMethod]
    public async Task GetCustomerInfoAsync_ReturnsOk_WhenNameIdentifierExists()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();

        SetUser(new Claim(ClaimTypes.NameIdentifier, uuid));

        var customer = Customer.Restore(
            uuid,
            "山田太郎",
            "ヤマダタロウ",
            "東京都新宿区",
            "〇〇ビル101",
            "03-1234-5678",
            "test@example.com",
            "testuser",
            "passwordHash");

        _usecaseMock
            .Setup(x => x.ExecuteAsync(uuid))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.GetCustomerInfoAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        var ok = (OkObjectResult)result.Result!;
        Assert.IsNotNull(ok.Value);

        var response = (CustomerInfoResponse)ok.Value!;

        Assert.AreEqual(customer.CustomerUuid, response.CustomerUuid);
        Assert.AreEqual(customer.Name, response.Name);
        Assert.AreEqual(customer.NameKana, response.NameKana);
        Assert.AreEqual(customer.Address1, response.Address1);
        Assert.AreEqual(customer.Address2, response.Address2);
        Assert.AreEqual(customer.PhoneNumber, response.PhoneNumber);
        Assert.AreEqual(customer.MailAddress, response.MailAddress);
        Assert.AreEqual(customer.Username, response.Username);

        _usecaseMock.Verify(x => x.ExecuteAsync(uuid), Times.Once);
        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 正常系：subクレームからUUIDを取得できる場合
    /// </summary>
    [TestMethod]
    public async Task GetCustomerInfoAsync_ReturnsOk_WhenSubClaimExists()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();

        SetUser(new Claim("sub", uuid));

        var customer = Customer.Restore(
            uuid,
            "山田太郎",
            "ヤマダタロウ",
            "東京都新宿区",
            "〇〇ビル101",
            "03-1234-5678",
            "test@example.com",
            "testuser",
            "passwordHash");

        _usecaseMock
            .Setup(x => x.ExecuteAsync(uuid))
            .ReturnsAsync(customer);

        // Act
        var result = await _controller.GetCustomerInfoAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        var ok = (OkObjectResult)result.Result!;
        Assert.IsNotNull(ok.Value);

        var response = (CustomerInfoResponse)ok.Value!;

        Assert.AreEqual(customer.CustomerUuid, response.CustomerUuid);
        Assert.AreEqual(customer.Name, response.Name);
        Assert.AreEqual(customer.NameKana, response.NameKana);
        Assert.AreEqual(customer.Address1, response.Address1);
        Assert.AreEqual(customer.Address2, response.Address2);
        Assert.AreEqual(customer.PhoneNumber, response.PhoneNumber);
        Assert.AreEqual(customer.MailAddress, response.MailAddress);
        Assert.AreEqual(customer.Username, response.Username);

        _usecaseMock.Verify(x => x.ExecuteAsync(uuid), Times.Once);
        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 異常系：認証情報が存在しない場合
    /// </summary>
    [TestMethod]
    public async Task GetCustomerInfoAsync_ReturnsUnauthorized_WhenNoClaimExists()
    {
        // Arrange
        SetUser();

        // Act
        var result = await _controller.GetCustomerInfoAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));

        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 異常系：顧客情報が存在しない場合
    /// </summary>
    [TestMethod]
    public async Task GetCustomerInfoAsync_ReturnsNotFound_WhenCustomerNotFound()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();

        SetUser(new Claim(ClaimTypes.NameIdentifier, uuid));

        _usecaseMock
            .Setup(x => x.ExecuteAsync(uuid))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _controller.GetCustomerInfoAsync();

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));

        _usecaseMock.Verify(x => x.ExecuteAsync(uuid), Times.Once);
        _usecaseMock.VerifyNoOtherCalls();
    }
}