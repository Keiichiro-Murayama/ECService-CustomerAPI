using System.Security.Claims;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

/// <summary>
/// 購入履歴取得Controllerの単体テスト
/// </summary>
[TestClass]
public class GetOrderHistoryControllerTests
{
    private Mock<IGetOrderHistoriesUsecase> _usecaseMock = null!;
    private GetOrderHistoriesViewModelAdapter _adapter = null!;
    private GetOrderHistoryController _controller = null!;

    /// <summary>
    /// テストの事前準備
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock =
            new Mock<IGetOrderHistoriesUsecase>();

        _adapter =
            new GetOrderHistoriesViewModelAdapter();

        _controller =
            new GetOrderHistoryController(
                _usecaseMock.Object,
                _adapter);
    }

    /// <summary>
    /// Controllerへ認証情報を設定する
    /// </summary>
    /// <param name="claims">認証情報</param>
    private void SetUser(params Claim[] claims)
    {
        var identity =
            new ClaimsIdentity(
                claims,
                "TestAuthentication");

        _controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User =
                            new ClaimsPrincipal(
                                identity)
                    }
            };
    }

    /// <summary>
    /// NameIdentifierから顧客UUIDを取得できる場合、
    /// 購入履歴一覧と200を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-006 NameIdentifierが存在する場合、購入履歴一覧と200を返す")]
    public async Task GetOrderHistoriesAsync_NameIdentifierが存在する場合_購入履歴一覧と200を返す()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440000";

        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        var orderDate =
            new DateTimeOffset(
                2026,
                7,
                20,
                10,
                30,
                0,
                TimeSpan.Zero);

        var order =
            Orders.Restore(
                1,
                "11111111-1111-1111-1111-111111111111",
                orderDate,
                3000,
                customerUuid,
                1,
                1,
                new List<OrderDetail>());

        var orders =
            new List<Orders>
            {
                order
            };

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    customerUuid))
            .ReturnsAsync(orders);

        // Act
        var result =
            await _controller
                .GetOrderHistoriesAsync();

        // Assert
        Assert.IsInstanceOfType<
            OkObjectResult>(result.Result);

        var okResult =
            (OkObjectResult)result.Result!;

        Assert.IsInstanceOfType<
            List<OrderResponse>>(okResult.Value);

        var responses =
            (List<OrderResponse>)okResult.Value!;

        Assert.HasCount(1, responses);

        Assert.AreEqual(
            order.Id,
            responses[0].OrderId);

        Assert.AreEqual(
            order.OrderUuid,
            responses[0].OrderUuid);

        Assert.AreEqual(
            order.OrderDate,
            responses[0].OrderDate);

        Assert.AreEqual(
            order.AmountTotal,
            responses[0].AmountTotal);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    customerUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// NameIdentifierがなくsubが存在する場合、
    /// subから顧客UUIDを取得して200を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-007 subが存在する場合、購入履歴一覧と200を返す")]
    public async Task GetOrderHistoriesAsync_Subが存在する場合_購入履歴一覧と200を返す()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440001";

        SetUser(
            new Claim(
                "sub",
                customerUuid));

        var order =
            Orders.Restore(
                2,
                "22222222-2222-2222-2222-222222222222",
                new DateTimeOffset(
                    2026,
                    7,
                    21,
                    15,
                    0,
                    0,
                    TimeSpan.Zero),
                5000,
                customerUuid,
                1,
                1,
                new List<OrderDetail>());

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    customerUuid))
            .ReturnsAsync(
                new List<Orders>
                {
                    order
                });

        // Act
        var result =
            await _controller
                .GetOrderHistoriesAsync();

        // Assert
        Assert.IsInstanceOfType<
            OkObjectResult>(result.Result);

        var okResult =
            (OkObjectResult)result.Result!;

        Assert.IsInstanceOfType<
            List<OrderResponse>>(okResult.Value);

        var responses =
            (List<OrderResponse>)okResult.Value!;

        Assert.HasCount(1, responses);

        Assert.AreEqual(
            order.OrderUuid,
            responses[0].OrderUuid);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    customerUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 顧客UUIDを取得できない場合、
    /// 401を返してUsecaseを呼び出さないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-008 認証情報が存在しない場合、401を返す")]
    public async Task GetOrderHistoriesAsync_認証情報が存在しない場合_401を返す()
    {
        // Arrange
        SetUser();

        // Act
        var result =
            await _controller
                .GetOrderHistoriesAsync();

        // Assert
        Assert.IsInstanceOfType<
            UnauthorizedObjectResult>(
                result.Result);

        var unauthorizedResult =
            (UnauthorizedObjectResult)
                result.Result!;

        Assert.IsNotNull(
            unauthorizedResult.Value);

        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 購入履歴が存在しない場合、
    /// 空の一覧と200を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-009 購入履歴が存在しない場合、空の一覧と200を返す")]
    public async Task GetOrderHistoriesAsync_購入履歴が存在しない場合_空の一覧と200を返す()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440099";

        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    customerUuid))
            .ReturnsAsync(
                new List<Orders>());

        // Act
        var result =
            await _controller
                .GetOrderHistoriesAsync();

        // Assert
        Assert.IsInstanceOfType<
            OkObjectResult>(result.Result);

        var okResult =
            (OkObjectResult)result.Result!;

        Assert.IsInstanceOfType<
            List<OrderResponse>>(okResult.Value);

        var responses =
            (List<OrderResponse>)okResult.Value!;

        Assert.IsEmpty(responses);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    customerUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Usecaseで例外が発生した場合、
    /// 例外が呼び出し元へ通知されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-010 Usecaseで例外が発生した場合、例外を通知する")]
    public async Task GetOrderHistoriesAsync_Usecaseで例外が発生した場合_例外を通知する()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440000";

        const string errorMessage =
            "購入履歴の取得に失敗しました。";

        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    customerUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    errorMessage));

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                () =>
                    _controller
                        .GetOrderHistoriesAsync());

        // Assert
        Assert.AreEqual(
            errorMessage,
            exception.Message);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    customerUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
    }
}