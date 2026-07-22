using ECService_CustomerAPI.Application.Authentications;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Application.Tests.Usecases;

[TestClass]
public class LoginUsecaseTests
{
    private Mock<ICustomerRepository> _customerRepositoryMock = null!;
    private Mock<IPasswordService> _passwordServiceMock = null!;
    private Mock<IJwtTokenProvider> _jwtTokenProviderMock = null!;
    private LoginUsecase _usecase = null!;

    private const string TestEmail = "LoginTest@example.com";
    private const string TestPassword = "LoginTestPass";

    [TestInitialize]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _jwtTokenProviderMock = new Mock<IJwtTokenProvider>();

        _usecase = new LoginUsecase(
            _customerRepositoryMock.Object,
            _passwordServiceMock.Object,
            _jwtTokenProviderMock.Object);
    }

    /// <summary>
    /// ログイン成功時にアクセストークンと顧客情報を返すこと
    /// </summary>
    [TestMethod(DisplayName = "ログイン成功時にアクセストークンと顧客情報を返すこと")]
    public async Task ExecuteAsync_ReturnAccessTokenAndCustomer_WhenLoginSucceeds()
    {
        // Arrange
        var customer = CreateCustomer();
        const string accessToken = "access-token";

        _customerRepositoryMock
            .Setup(x => x.FindByMailAddressAsync(TestEmail))
            .ReturnsAsync(customer);
        _passwordServiceMock
            .Setup(x => x.Verify(customer.PasswordHash, TestPassword))
            .Returns(true);
        _jwtTokenProviderMock
            .Setup(x => x.IssueAccessToken(customer, null))
            .Returns(accessToken);

        // Act
        var result = await _usecase.ExecuteAsync((TestEmail, TestPassword));

        // Assert
        Assert.AreEqual(accessToken, result.AccessToken);
        Assert.AreSame(customer, result.Customer);

        _customerRepositoryMock.Verify(x => x.FindByMailAddressAsync(TestEmail), Times.Once);
        _passwordServiceMock.Verify(x => x.Verify(customer.PasswordHash, TestPassword), Times.Once);
        _jwtTokenProviderMock.Verify(x => x.IssueAccessToken(customer, null), Times.Once);
        _customerRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _jwtTokenProviderMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// メールアドレスがnullの場合に認証例外をスローすること
    /// </summary>
    [TestMethod(DisplayName = "メールアドレスがnullの場合に認証例外をスローすること")]
    public async Task ExecuteAsync_ThrowAuthenticationException_WhenMailAddressIsNull()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(x => x.FindByMailAddressAsync(null!))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _usecase.ExecuteAsync((null!, TestPassword)));

        Assert.AreEqual("メールアドレスが正しくありません。", exception.Message);
        Assert.AreEqual("AuthenticationFailed", exception.ErrorCode);

        _customerRepositoryMock.Verify(x => x.FindByMailAddressAsync(null!), Times.Once);
        _customerRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _jwtTokenProviderMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 存在しないメールアドレスの場合に認証例外をスローすること
    /// </summary>
    [TestMethod(DisplayName = "存在しないメールアドレスの場合に認証例外をスローすること")]
    public async Task ExecuteAsync_ThrowAuthenticationException_WhenMailAddressDoesNotExist()
    {
        // Arrange
        const string unexistMailAddress = "UnexistTest@example.com";

        _customerRepositoryMock
            .Setup(x => x.FindByMailAddressAsync(unexistMailAddress))
            .ReturnsAsync((Customer?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _usecase.ExecuteAsync((unexistMailAddress, "UnexistPass")));

        Assert.AreEqual("メールアドレスが正しくありません。", exception.Message);
        Assert.AreEqual("AuthenticationFailed", exception.ErrorCode);

        _customerRepositoryMock.Verify(x => x.FindByMailAddressAsync(unexistMailAddress), Times.Once);
        _customerRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _jwtTokenProviderMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// パスワードがnullの場合に認証例外をスローすること
    /// </summary>
    [TestMethod(DisplayName = "パスワードがnullの場合に認証例外をスローすること")]
    public async Task ExecuteAsync_ThrowAuthenticationException_WhenPasswordIsNull()
    {
        // Arrange
        var customer = CreateCustomer();

        _customerRepositoryMock
            .Setup(x => x.FindByMailAddressAsync(TestEmail))
            .ReturnsAsync(customer);
        _passwordServiceMock
            .Setup(x => x.Verify(customer.PasswordHash, null!))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _usecase.ExecuteAsync((TestEmail, null!)));

        Assert.AreEqual("パスワードが正しくありません。", exception.Message);
        Assert.AreEqual("AuthenticationFailed", exception.ErrorCode);

        _customerRepositoryMock.Verify(x => x.FindByMailAddressAsync(TestEmail), Times.Once);
        _passwordServiceMock.Verify(x => x.Verify(customer.PasswordHash, null!), Times.Once);
        _customerRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _jwtTokenProviderMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// パスワードが間違っている場合に認証例外をスローすること
    /// </summary>
    [TestMethod(DisplayName = "パスワードが間違っている場合に認証例外をスローすること")]
    public async Task ExecuteAsync_ThrowAuthenticationException_WhenPasswordIsWrong()
    {
        // Arrange
        var customer = CreateCustomer();

        _customerRepositoryMock
            .Setup(x => x.FindByMailAddressAsync(TestEmail))
            .ReturnsAsync(customer);
        _passwordServiceMock
            .Setup(x => x.Verify(customer.PasswordHash, "WrongPass"))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AuthenticationException>(
            () => _usecase.ExecuteAsync((TestEmail, "WrongPass")));

        Assert.AreEqual("パスワードが正しくありません。", exception.Message);
        Assert.AreEqual("AuthenticationFailed", exception.ErrorCode);

        _customerRepositoryMock.Verify(x => x.FindByMailAddressAsync(TestEmail), Times.Once);
        _passwordServiceMock.Verify(x => x.Verify(customer.PasswordHash, "WrongPass"), Times.Once);
        _customerRepositoryMock.VerifyNoOtherCalls();
        _passwordServiceMock.VerifyNoOtherCalls();
        _jwtTokenProviderMock.VerifyNoOtherCalls();
    }

    private static Customer CreateCustomer()
    {
        return Customer.Restore(
            Guid.NewGuid().ToString(),
            "LoginTest",
            "ログインテスト",
            "Tokyo",
            "Chiyoda",
            "03-1234-5678",
            TestEmail,
            "LoginTestUser",
            "hashed-password");
    }
}