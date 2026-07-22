using System.Security.Claims;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

/// <summary>
/// 商品購入Controllerの単体テスト
/// Authorize属性による認証フィルターの動作は結合テストで確認する。
/// </summary>
[TestClass]
public class PurchaseControllerTests
{
    private const string CustomerUuid =
        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

    private const string OrderUuid =
        "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

    private const string PenUuid =
        "3fd7d44e-7cac-444b-b747-44eb988a0421";

    private const string NoteUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    private Mock<IPurchaseUsecase> _purchaseUsecase = null!;
    private Mock<ILogger<PurchaseController>> _logger = null!;
    private PurchaseController _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _purchaseUsecase =
            new Mock<IPurchaseUsecase>(MockBehavior.Strict);

        // ILoggerは拡張メソッド経由でLogが呼ばれるためLooseを使用する。
        _logger =
            new Mock<ILogger<PurchaseController>>(
                MockBehavior.Loose);

        _sut = new PurchaseController(
            _purchaseUsecase.Object,
            _logger.Object);

        SetUser();
    }

    /// <summary>
    /// PUR-PRE-001
    /// </summary>
    [TestMethod]
    public async Task PurchaseAsync_正常なリクエストの場合_200と注文UUIDを返す()
    {
        // Arrange
        SetUser(
            new Claim(
                JwtRegisteredClaimNames.Sub,
                CustomerUuid));

        _purchaseUsecase
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    CustomerUuid,
                    1,
                    It.Is<List<(string ProductUuid, int Quantity)>>(
                        items =>
                            items.Count == 2
                            && items[0].ProductUuid == PenUuid
                            && items[0].Quantity == 2
                            && items[1].ProductUuid == NoteUuid
                            && items[1].Quantity == 3)))
            .ReturnsAsync(OrderUuid);

        var request = CreateValidRequest();

        // Act
        var actionResult =
            await _sut.PurchaseAsync(request);

        // Assert
        var okResult =
            actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult!.StatusCode ?? 0);

        var response =
            okResult.Value as PurchaseResponse;

        Assert.IsNotNull(response);
        Assert.AreEqual(OrderUuid, response!.OrderUuid);
        Assert.AreEqual(
            "購入が完了しました。",
            response.Message);

        _purchaseUsecase.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    CustomerUuid,
                    1,
                    It.IsAny<
                        List<(string ProductUuid, int Quantity)>>()),
            Times.Once);
    }

    /// <summary>
    /// PUR-PRE-002
    /// </summary>
    [TestMethod]
    public async Task PurchaseAsync_必須項目不足または支払い方法ID不正の場合_400を返す()
    {
        // Arrange
        var testCases =
            new PurchaseRequest?[]
            {
                null,
                new PurchaseRequest
                {
                    PaymentMethodId = string.Empty,
                    Items =
                        new List<PurchaseItemRequest>
                        {
                            new()
                            {
                                ProductUuid = PenUuid,
                                Quantity = 1
                            }
                        }
                },
                new PurchaseRequest
                {
                    PaymentMethodId = "abc",
                    Items =
                        new List<PurchaseItemRequest>
                        {
                            new()
                            {
                                ProductUuid = PenUuid,
                                Quantity = 1
                            }
                        }
                },
                new PurchaseRequest
                {
                    PaymentMethodId = "1",
                    Items = null!
                },
                new PurchaseRequest
                {
                    PaymentMethodId = "1",
                    Items =
                        new List<PurchaseItemRequest>()
                },
                new PurchaseRequest
                {
                    PaymentMethodId = "1",
                    Items =
                        new List<PurchaseItemRequest>
                        {
                            new()
                            {
                                ProductUuid = string.Empty,
                                Quantity = 1
                            }
                        }
                }
            };

        foreach (var request in testCases)
        {
            // Act
            var actionResult =
                await _sut.PurchaseAsync(request);

            // Assert
            var badRequest =
                actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(
                StatusCodes.Status400BadRequest,
                badRequest!.StatusCode ?? 0);

            Assert.AreEqual(
                "支払い方法、または購入商品を選択してください。",
                GetMessage(badRequest.Value));
        }

        _purchaseUsecase.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<
                        List<(string ProductUuid, int Quantity)>>()),
            Times.Never);
    }

    /// <summary>
    /// PUR-PRE-003
    /// </summary>
    [TestMethod]
    public async Task PurchaseAsync_顧客UUIDクレームがない場合_401を返す()
    {
        // Arrange
        SetUser();

        var request =
            new PurchaseRequest
            {
                PaymentMethodId = "1",
                Items =
                    new List<PurchaseItemRequest>
                    {
                        new()
                        {
                            ProductUuid = PenUuid,
                            Quantity = 1
                        }
                    }
            };

        // Act
        var actionResult =
            await _sut.PurchaseAsync(request);

        // Assert
        var unauthorized =
            actionResult.Result as UnauthorizedObjectResult;

        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(
            StatusCodes.Status401Unauthorized,
            unauthorized!.StatusCode ?? 0);

        Assert.AreEqual(
            "認証が必要です。ログインしてください。",
            GetMessage(unauthorized.Value));

        _purchaseUsecase.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<
                        List<(string ProductUuid, int Quantity)>>()),
            Times.Never);
    }

    /// <summary>
    /// PUR-PRE-004
    /// </summary>
    [TestMethod]
    public async Task PurchaseAsync_業務例外が発生した場合_400または404へ変換する()
    {
        // Arrange
        SetUser(
            new Claim(
                JwtRegisteredClaimNames.Sub,
                CustomerUuid));

        const string domainMessage =
            "申し訳ありませんが、商品「高級ボールペン」の在庫が不足しています。";

        _purchaseUsecase
            .SetupSequence(usecase =>
                usecase.ExecuteAsync(
                    CustomerUuid,
                    1,
                    It.IsAny<
                        List<(string ProductUuid, int Quantity)>>()))
            .ThrowsAsync(
                new DomainException(
                    domainMessage,
                    "quantity"))
            .ThrowsAsync(
                new NotFoundException(
                    "商品が見つかりません。"));

        var request =
            new PurchaseRequest
            {
                PaymentMethodId = "1",
                Items =
                    new List<PurchaseItemRequest>
                    {
                        new()
                        {
                            ProductUuid = PenUuid,
                            Quantity = 1
                        }
                    }
            };

        // Act・Assert：DomainException
        var badRequestAction =
            await _sut.PurchaseAsync(request);

        var badRequest =
            badRequestAction.Result
                as BadRequestObjectResult;

        Assert.IsNotNull(badRequest);
        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest!.StatusCode ?? 0);

        Assert.AreEqual(
            domainMessage,
            GetMessage(badRequest.Value));

        // Act・Assert：NotFoundException
        var notFoundAction =
            await _sut.PurchaseAsync(request);

        var notFound =
            notFoundAction.Result
                as NotFoundObjectResult;

        Assert.IsNotNull(notFound);
        Assert.AreEqual(
            StatusCodes.Status404NotFound,
            notFound!.StatusCode ?? 0);

        Assert.AreEqual(
            "指定されたリソースが見つかりません。",
            GetMessage(notFound.Value));
    }

    /// <summary>
    /// PUR-PRE-005
    /// </summary>
    [TestMethod]
    public async Task PurchaseAsync_内部エラーが発生した場合_500の共通メッセージを返す()
    {
        // Arrange
        SetUser(
            new Claim(
                JwtRegisteredClaimNames.Sub,
                CustomerUuid));

        _purchaseUsecase
            .SetupSequence(usecase =>
                usecase.ExecuteAsync(
                    CustomerUuid,
                    1,
                    It.IsAny<
                        List<(string ProductUuid, int Quantity)>>()))
            .ThrowsAsync(
                new InternalException(
                    "DBエラー"))
            .ThrowsAsync(
                new Exception(
                    "想定外エラー"));

        var request =
            new PurchaseRequest
            {
                PaymentMethodId = "1",
                Items =
                    new List<PurchaseItemRequest>
                    {
                        new()
                        {
                            ProductUuid = PenUuid,
                            Quantity = 1
                        }
                    }
            };

        for (var index = 0; index < 2; index++)
        {
            // Act
            var actionResult =
                await _sut.PurchaseAsync(request);

            // Assert
            var internalServerError =
                actionResult.Result as ObjectResult;

            Assert.IsNotNull(internalServerError);
            Assert.AreEqual(
                StatusCodes.Status500InternalServerError,
                internalServerError!.StatusCode ?? 0);

            Assert.AreEqual(
                "サーバー内部で予期せぬエラーが発生しました。",
                GetMessage(internalServerError.Value));
        }
    }

    private static PurchaseRequest CreateValidRequest()
    {
        return new PurchaseRequest
        {
            PaymentMethodId = "1",
            Items =
                new List<PurchaseItemRequest>
                {
                    new()
                    {
                        ProductUuid = PenUuid,
                        Quantity = 2
                    },
                    new()
                    {
                        ProductUuid = NoteUuid,
                        Quantity = 3
                    }
                }
        };
    }

    private void SetUser(params Claim[] claims)
    {
        var identity =
            new ClaimsIdentity(
                claims,
                authenticationType: "UnitTest");

        var httpContext =
            new DefaultHttpContext
            {
                User =
                    new ClaimsPrincipal(identity)
            };

        _sut.ControllerContext =
            new ControllerContext
            {
                HttpContext = httpContext
            };
    }

    private static string? GetMessage(object? responseBody)
    {
        if (responseBody == null)
        {
            return null;
        }

        var messageProperty =
            responseBody
                .GetType()
                .GetProperty("message");

        return messageProperty?
            .GetValue(responseBody)?
            .ToString();
    }
}