using System.ComponentModel.DataAnnotations;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

[TestClass]
public class AuthControllerTests
{
    private Mock<ILoginUsecase> _loginUsecaseMock = null!;
    private AuthController _controller = null!;

    private const string TestEmail = "LoginTest@example.com";
    private const string TestPassword = "LoginTestPass";

    [TestInitialize]
    public void Setup()
    {
        _loginUsecaseMock = new Mock<ILoginUsecase>();

        _controller = new AuthController(_loginUsecaseMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [TestMethod(DisplayName = "UT-LGI-010: ログイン正常系でTokenResponseを返すこと")]
    public async Task Login_ReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        const string accessToken = "token-for-login-test";
        var customer = CreateCustomer();
        var input = (MailAddress: TestEmail, Password: TestPassword);

        _loginUsecaseMock
            .Setup(x => x.ExecuteAsync(input))
            .ReturnsAsync((accessToken, customer));

        var request = new LoginRequest
        {
            EmailAddress = TestEmail,
            Password = TestPassword
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

        var response = okResult.Value as TokenResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual(accessToken, response.Token);
        Assert.AreEqual(customer.CustomerUuid, response.CustomerUuid);
        Assert.AreEqual(customer.Username, response.Username);
        Assert.AreEqual("ログインに成功しました。", response.Message);

        _loginUsecaseMock.Verify(x => x.ExecuteAsync(input), Times.Once);
        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-011: メールアドレス未入力時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenEmailAddressIsNull()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = null!,
            Password = TestPassword
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("メールアドレスを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-012: パスワード未入力時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenPasswordIsNull()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = TestEmail,
            Password = null!
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("パスワードを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-013: 存在しないメールアドレスで認証失敗した場合")]
    public async Task Login_ReturnUnauthorized_WhenEmailAddressDoesNotExist()
    {
        // Arrange
        const string unexistEmail = "Unexist@example.com";
        var input = (MailAddress: unexistEmail, Password: "UnexistPass");

        _loginUsecaseMock
            .Setup(x => x.ExecuteAsync(input))
            .ThrowsAsync(new AuthenticationException("メールアドレスが正しくありません。"));

        var request = new LoginRequest
        {
            EmailAddress = unexistEmail,
            Password = "UnexistPass"
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorized = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        Assert.AreEqual("メールアドレスが正しくありません。", ExtractPropertyValue(unauthorized.Value, "message"));

        _loginUsecaseMock.Verify(x => x.ExecuteAsync(input), Times.Once);
        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-014: メールアドレス形式不正時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenEmailAddressFormatIsInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = "LoginTestexample.com",
            Password = TestPassword
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("メールアドレスを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-015: メールアドレス最小文字列長未満時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenEmailAddressIsShorterThanMinimumLength()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = "a@c",
            Password = TestPassword
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("メールアドレスを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-016: メールアドレス最大文字列長超過時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenEmailAddressExceedsMaximumLength()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = $"LoginTest{new string('A', 100)}@example.com",
            Password = TestPassword
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("メールアドレスを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-017: パスワード誤りで認証失敗した場合")]
    public async Task Login_ReturnUnauthorized_WhenPasswordIsWrong()
    {
        // Arrange
        const string wrongPassword = "WrongPass";
        var input = (MailAddress: TestEmail, Password: wrongPassword);

        _loginUsecaseMock
            .Setup(x => x.ExecuteAsync(input))
            .ThrowsAsync(new AuthenticationException("パスワードが正しくありません。"));

        var request = new LoginRequest
        {
            EmailAddress = TestEmail,
            Password = wrongPassword
        };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorized = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(StatusCodes.Status401Unauthorized, unauthorized.StatusCode);
        Assert.AreEqual("パスワードが正しくありません。", ExtractPropertyValue(unauthorized.Value, "message"));

        _loginUsecaseMock.Verify(x => x.ExecuteAsync(input), Times.Once);
        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-018: パスワード最小文字列長未満時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenPasswordIsShorterThanMinimumLength()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = TestEmail,
            Password = "aaaa"
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("パスワードを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    [TestMethod(DisplayName = "UT-LGI-019: パスワード最大文字列長超過時に400を返すこと")]
    public async Task Login_ReturnBadRequest_WhenPasswordExceedsMaximumLength()
    {
        // Arrange
        var request = new LoginRequest
        {
            EmailAddress = TestEmail,
            Password = $"LoginTestPass{new string('A', 20)}"
        };
        ValidateModel(request);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.AreEqual("パスワードを正しく入力してください。", ExtractPropertyValue(badRequest.Value, "message"));

        _loginUsecaseMock.VerifyNoOtherCalls();
    }

    //ログアウトのテストは、Cookieの削除を確認するだけなので、ユースケースのモックは不要
    [TestMethod(DisplayName = "UT-LGO-010: ログアウト時にCookieが削除されること")]
    public void Logout_ClearsAuthCookie()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

        var setCookieHeader = _controller.Response.Headers.SetCookie.ToString();
        Assert.Contains("access_token=;", setCookieHeader);

        var message = ExtractPropertyValue(okResult.Value, "Message");
        Assert.AreEqual("ログアウトしました。", message);
    }

    private void ValidateModel(LoginRequest request)
    {
        _controller.ModelState.Clear();

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request, serviceProvider: null, items: null);

        Validator.TryValidateObject(request, context, validationResults, validateAllProperties: true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames.DefaultIfEmpty(string.Empty))
            {
                _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage ?? "バリデーションエラー");
            }
        }
    }

    private static string? ExtractPropertyValue(object? value, string propertyName)
    {
        if (value == null)
        {
            return null;
        }

        var property = value.GetType().GetProperty(propertyName);
        return property?.GetValue(value)?.ToString();
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