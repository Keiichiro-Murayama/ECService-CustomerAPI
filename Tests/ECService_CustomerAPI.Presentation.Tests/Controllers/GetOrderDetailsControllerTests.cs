using System.Security.Claims;
using System.Text.Json;
using ECService_CustomerAPI.Application.Usecases.Interfaces;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Presentation.Adapters;
using ECService_CustomerAPI.Presentation.Controllers;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Controllers;

/// <summary>
/// 注文明細取得Controllerの単体テスト
/// </summary>
[TestClass]
public class GetOrderDetailsControllerTests
{
    private Mock<IGetOrderDetailsUsecase> _usecaseMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private GetOrderDetailsViewModelAdapter _adapter = null!;
    private GetOrderDetailsController _controller = null!;

    /// <summary>
    /// テストの事前準備
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock =
            new Mock<IGetOrderDetailsUsecase>();

        _productRepositoryMock =
            new Mock<IProductRepository>();

        _adapter =
            new GetOrderDetailsViewModelAdapter(
                _productRepositoryMock.Object);

        _controller =
            new GetOrderDetailsController(
                _usecaseMock.Object,
                _adapter);
    }

    /// <summary>
    /// 有効な注文UUIDで注文明細が存在する場合、
    /// 注文明細一覧と200を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-007 有効な注文UUIDで本人の注文明細が存在する場合、注文明細一覧と200を返す")]
    public async Task GetOrderDetailsAsync_本人の注文明細が存在する場合_注文明細一覧と200を返す()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        const string productUuid =
            "22222222-2222-2222-2222-222222222222";

        //石原:追加 ログイン中の顧客UUIDを認証クレームへ設定
        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        var orderDetails =
            new List<OrderDetail>
            {
                OrderDetail.Restore(
                    productUuid,
                    2)
            };

        //石原:変更 注文UUIDと顧客UUIDの両方がUsecaseへ渡されるよう変更
        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid))
            .ReturnsAsync(orderDetails);

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectNameByProductUuidAsync(
                    productUuid))
            .ReturnsAsync("ゲーミングマウス");

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(
                    productUuid))
            .ReturnsAsync(3800);

        // Act
        var result =
            await _controller.GetOrderDetailsAsync(
                orderUuid);

        // Assert
        Assert.IsInstanceOfType<OkObjectResult>(
            result.Result);

        var okResult =
            (OkObjectResult)result.Result!;

        Assert.IsInstanceOfType<
            List<OrderDetailResponse>>(
                okResult.Value);

        var responses =
            (List<OrderDetailResponse>)okResult.Value!;

        Assert.HasCount(1, responses);

        Assert.AreEqual(
            productUuid,
            responses[0].ProductUuid);

        Assert.AreEqual(
            "ゲーミングマウス",
            responses[0].ProductName);

        Assert.AreEqual(
            3800,
            responses[0].Price);

        Assert.AreEqual(
            2,
            responses[0].Quantity);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectNameByProductUuidAsync(
                    productUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectPriceByProductUuidAsync(
                    productUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 注文UUIDの形式が不正な場合、
    /// 400を返してUsecaseを呼び出さないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-008 注文UUIDの形式が不正な場合、400を返す")]
    public async Task GetOrderDetailsAsync_注文UUIDの形式が不正な場合_400を返す()
    {
        // Arrange
        const string invalidOrderUuid =
            "invalid-order-uuid";

        // Act
        var result =
            await _controller.GetOrderDetailsAsync(
                invalidOrderUuid);

        // Assert
        Assert.IsInstanceOfType<
            BadRequestObjectResult>(
                result.Result);

        var badRequestResult =
            (BadRequestObjectResult)result.Result!;

        var responseJson =
            JsonSerializer.Serialize(
                badRequestResult.Value);

        using var jsonDocument =
            JsonDocument.Parse(
                responseJson);

        var message =
            jsonDocument.RootElement
                .GetProperty("message")
                .GetString();

        Assert.AreEqual(
            "注文UUIDの形式が正しくありません。",
            message);

        _usecaseMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 本人の注文が存在しない場合、
    /// 404を返してAdapterを実行しないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-009 本人の注文明細が存在しない場合、404を返す")]
    public async Task GetOrderDetailsAsync_本人の注文明細が存在しない場合_404を返す()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid))
            .ReturnsAsync(
                new List<OrderDetail>());

        // Act
        var result =
            await _controller.GetOrderDetailsAsync(
                orderUuid);

        // Assert
        Assert.IsInstanceOfType<
            NotFoundObjectResult>(
                result.Result);

        var notFoundResult =
            (NotFoundObjectResult)result.Result!;

        var responseJson =
            JsonSerializer.Serialize(
                notFoundResult.Value);

        using var jsonDocument =
            JsonDocument.Parse(
                responseJson);

        var message =
            jsonDocument.RootElement
                .GetProperty("message")
                .GetString();

        Assert.AreEqual(
            "指定した注文の明細が見つかりませんでした。",
            message);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Usecaseで例外が発生した場合、
    /// 例外が呼び出し元へ通知されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-010 Usecaseで例外が発生した場合、例外を通知する")]
    public async Task GetOrderDetailsAsync_Usecaseで例外が発生した場合_例外を通知する()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        const string errorMessage =
            "注文明細の取得に失敗しました。";

        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    errorMessage));

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                () =>
                    _controller.GetOrderDetailsAsync(
                        orderUuid));

        // Assert
        Assert.AreEqual(
            errorMessage,
            exception.Message);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        _usecaseMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Adapterで商品情報を取得中に例外が発生した場合、
    /// 例外が呼び出し元へ通知されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-011 Adapterで例外が発生した場合、例外を通知する")]
    public async Task GetOrderDetailsAsync_Adapterで例外が発生した場合_例外を通知する()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        const string productUuid =
            "22222222-2222-2222-2222-222222222222";

        const string errorMessage =
            "商品情報の取得に失敗しました。";

        SetUser(
            new Claim(
                ClaimTypes.NameIdentifier,
                customerUuid));

        var orderDetails =
            new List<OrderDetail>
            {
                OrderDetail.Restore(
                    productUuid,
                    2)
            };

        _usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid))
            .ReturnsAsync(orderDetails);

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectNameByProductUuidAsync(
                    productUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    errorMessage));

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                () =>
                    _controller.GetOrderDetailsAsync(
                        orderUuid));

        // Assert
        Assert.AreEqual(
            errorMessage,
            exception.Message);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectNameByProductUuidAsync(
                    productUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectPriceByProductUuidAsync(
                    It.IsAny<string>()),
            Times.Never);

        _usecaseMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 顧客UUIDを認証情報から取得できない場合、
    /// 401を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-012 顧客UUIDを取得できない場合、401を返す")]
    public async Task GetOrderDetailsAsync_顧客UUIDを取得できない場合_401を返す()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        //石原:追加 顧客UUIDを持たない認証情報を設定
        SetUser();

        // Act
        var result =
            await _controller.GetOrderDetailsAsync(
                orderUuid);

        // Assert
        Assert.IsInstanceOfType<
            UnauthorizedObjectResult>(
                result.Result);

        var unauthorizedResult =
            (UnauthorizedObjectResult)result.Result!;

        var responseJson =
            JsonSerializer.Serialize(
                unauthorizedResult.Value);

        using var jsonDocument =
            JsonDocument.Parse(
                responseJson);

        var message =
            jsonDocument.RootElement
                .GetProperty("message")
                .GetString();

        Assert.AreEqual(
            "顧客情報を取得できませんでした。",
            message);

        _usecaseMock.VerifyNoOtherCalls();
        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Controllerへ認証情報を設定する
    /// </summary>
    /// <param name="claims">認証クレーム</param>
    //石原:追加 注文明細取得テストでログイン顧客を再現するための共通処理
    private void SetUser(
        params Claim[] claims)
    {
        var identity =
            new ClaimsIdentity(
                claims,
                "TestAuthentication");

        var principal =
            new ClaimsPrincipal(
                identity);

        _controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User = principal
                    }
            };
    }
}