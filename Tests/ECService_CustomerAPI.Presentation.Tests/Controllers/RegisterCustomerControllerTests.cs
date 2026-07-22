using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegisterCustomerInput = (
    string Name,
    string NameKana,
    string Address1,
    string Address2,
    string PhoneNumber,
    string MailAddress,
    string Username,
    string Password);

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

/// <summary>
/// 顧客アカウント登録Controllerの単体テスト。
/// HTTPルーティング、モデルバインド、DIは結合テストで確認する。
/// </summary>
[TestClass]
public class RegisterCustomerControllerTests
{
    private const string CustomerUuid =
        "11111111-1111-1111-1111-111111111111";

    private Mock<IRegisterCustomerUsecase>
        _registerCustomerUsecase = null!;

    private RegisterCustomerController _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _registerCustomerUsecase =
            new Mock<IRegisterCustomerUsecase>(
                MockBehavior.Strict);

        _sut = new RegisterCustomerController(
            _registerCustomerUsecase.Object);
    }

    /// <summary>
    /// UT-RC-021
    /// </summary>
    [TestMethod]
    public async Task RegisterCustomerAsync_リクエストがnullの場合_400を返してUsecaseを呼ばない()
    {
        // Act
        var result =
            await _sut.RegisterCustomerAsync(null);

        // Assert
        var badRequest =
            result as BadRequestObjectResult;

        Assert.IsNotNull(badRequest);
        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest!.StatusCode ?? 0);
        Assert.AreEqual(
            "リクエストボディが正しくありません。",
            GetStringProperty(
                badRequest.Value,
                "message"));

        _registerCustomerUsecase.Verify(
            usecase => usecase.ExecuteAsync(
                It.IsAny<RegisterCustomerInput>()),
            Times.Never);
    }

    /// <summary>
    /// UT-RC-022
    /// </summary>
    [TestMethod]
    public async Task RegisterCustomerAsync_ModelStateにエラーが1件ある場合_詳細メッセージを400で返す()
    {
        // Arrange
        const string expectedMessage =
            "氏名カナは全角カナで入力し、スペースは文字と文字の間に1つだけ入力してください。";

        _sut.ModelState.AddModelError(
            "NameKana",
            expectedMessage);

        var request = CreateValidRequest();

        // Act
        var result =
            await _sut.RegisterCustomerAsync(request);

        // Assert
        var badRequest =
            result as BadRequestObjectResult;

        Assert.IsNotNull(badRequest);
        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest!.StatusCode ?? 0);
        Assert.AreEqual(
            expectedMessage,
            GetStringProperty(
                badRequest.Value,
                "message"));

        _registerCustomerUsecase.Verify(
            usecase => usecase.ExecuteAsync(
                It.IsAny<RegisterCustomerInput>()),
            Times.Never);
    }

    /// <summary>
    /// UT-RC-023
    /// </summary>
    [TestMethod]
    public async Task RegisterCustomerAsync_ModelStateに複数エラーがある場合_最初のメッセージを返す()
    {
        // Arrange
        const string firstMessage =
            "氏名のスペースは文字と文字の間に1つだけ入力してください。";

        _sut.ModelState.AddModelError(
            "ValidationErrors",
            firstMessage);
        _sut.ModelState.AddModelError(
            "ValidationErrors",
            "メールアドレスの形式が正しくありません。");

        var request = CreateValidRequest();

        // Act
        var result =
            await _sut.RegisterCustomerAsync(request);

        // Assert
        var badRequest =
            result as BadRequestObjectResult;

        Assert.IsNotNull(badRequest);
        Assert.AreEqual(
            firstMessage,
            GetStringProperty(
                badRequest!.Value,
                "message"));

        _registerCustomerUsecase.Verify(
            usecase => usecase.ExecuteAsync(
                It.IsAny<RegisterCustomerInput>()),
            Times.Never);
    }

    /// <summary>
    /// UT-RC-024
    /// </summary>
    [TestMethod]
    public async Task RegisterCustomerAsync_正常なリクエストの場合_8項目をUsecaseへ渡して201を返す()
    {
        // Arrange
        var request = CreateValidRequestWithSpace();

        _registerCustomerUsecase
            .Setup(usecase => usecase.ExecuteAsync(
                It.Is<RegisterCustomerInput>(
                    input =>
                        input.Name == request.Name &&
                        input.NameKana == request.NameKana &&
                        input.Address1 == request.Address1 &&
                        input.Address2 == request.Address2 &&
                        input.PhoneNumber ==
                            request.PhoneNumber &&
                        input.MailAddress ==
                            request.MailAddress &&
                        input.Username ==
                            request.AccountName &&
                        input.Password ==
                            request.Password)))
            .ReturnsAsync(CustomerUuid);

        // Act
        var result =
            await _sut.RegisterCustomerAsync(request);

        // Assert
        var createdResult =
            result as ObjectResult;

        Assert.IsNotNull(createdResult);
        Assert.AreEqual(
            StatusCodes.Status201Created,
            createdResult!.StatusCode ?? 0);
        Assert.AreEqual(
            CustomerUuid,
            GetStringProperty(
                createdResult.Value,
                "customerUuid"));
        Assert.AreEqual(
            "アカウント登録が完了しました。",
            GetStringProperty(
                createdResult.Value,
                "message"));

        _registerCustomerUsecase.Verify(
            usecase => usecase.ExecuteAsync(
                It.IsAny<RegisterCustomerInput>()),
            Times.Once);
    }

    /// <summary>
    /// UT-RC-025
    /// </summary>
    [TestMethod]
    public async Task RegisterCustomerAsync_ConflictExceptionが発生した場合_409と例外メッセージを返す()
    {
        // Arrange
        const string expectedMessage =
            "このアカウント名、メールアドレス、または電話番号は既に登録されています。";

        _registerCustomerUsecase
            .Setup(usecase => usecase.ExecuteAsync(
                It.IsAny<RegisterCustomerInput>()))
            .ThrowsAsync(
                new ConflictException(
                    expectedMessage));

        var request = CreateValidRequest();

        // Act
        var result =
            await _sut.RegisterCustomerAsync(request);

        // Assert
        var conflictResult =
            result as ConflictObjectResult;

        Assert.IsNotNull(conflictResult);
        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflictResult!.StatusCode ?? 0);
        Assert.AreEqual(
            expectedMessage,
            GetStringProperty(
                conflictResult.Value,
                "message"));
    }

    /// <summary>
    /// UT-RC-026
    /// </summary>
    [TestMethod]
    public async Task RegisterCustomerAsync_DomainExceptionが発生した場合_詳細メッセージまたは既定メッセージを400で返す()
    {
        // Arrange
        const string domainMessage =
            "氏名カナは全角カナで入力し、スペースは文字と文字の間に1つだけ入力してください。";

        _registerCustomerUsecase
            .SetupSequence(usecase =>
                usecase.ExecuteAsync(
                    It.IsAny<RegisterCustomerInput>()))
            .ThrowsAsync(
                new DomainException(
                    domainMessage,
                    "nameKana"))
            .ThrowsAsync(
                new EmptyMessageDomainException());

        var request = CreateValidRequest();

        // Act・Assert：詳細メッセージあり
        var detailResult =
            await _sut.RegisterCustomerAsync(request);

        var detailBadRequest =
            detailResult as BadRequestObjectResult;

        Assert.IsNotNull(detailBadRequest);
        Assert.AreEqual(
            domainMessage,
            GetStringProperty(
                detailBadRequest!.Value,
                "message"));

        // Act・Assert：メッセージが空
        var defaultResult =
            await _sut.RegisterCustomerAsync(request);

        var defaultBadRequest =
            defaultResult as BadRequestObjectResult;

        Assert.IsNotNull(defaultBadRequest);
        Assert.AreEqual(
            "入力値に不備があります。",
            GetStringProperty(
                defaultBadRequest!.Value,
                "message"));

        _registerCustomerUsecase.Verify(
            usecase => usecase.ExecuteAsync(
                It.IsAny<RegisterCustomerInput>()),
            Times.Exactly(2));
    }

    private static RegisterCustomerRequest CreateValidRequest()
    {
        return new RegisterCustomerRequest
        {
            Name = "山田太郎",
            NameKana = "ヤマダタロウ",
            Address1 = "東京都渋谷区1-11-11",
            Address2 = "マンション101号室",
            PhoneNumber = "03-1111-2222",
            MailAddress = "taro@example.com",
            AccountName = "taro123",
            Password = "Password123"
        };
    }

    private static RegisterCustomerRequest
        CreateValidRequestWithSpace()
    {
        var request = CreateValidRequest();
        request.Name = "山田 太郎";
        request.NameKana = "ヤマダ タロウ";

        return request;
    }

    private static string? GetStringProperty(
        object? responseBody,
        string propertyName)
    {
        if (responseBody is null)
        {
            return null;
        }

        return responseBody
            .GetType()
            .GetProperty(propertyName)?
            .GetValue(responseBody)?
            .ToString();
    }

    private sealed class EmptyMessageDomainException
        : DomainException
    {
        public override string Message =>
            string.Empty;
    }
}