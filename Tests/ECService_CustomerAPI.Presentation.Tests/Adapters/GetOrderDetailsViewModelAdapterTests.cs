using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Presentation.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Presentation.Tests.Adapters;

/// <summary>
/// 注文明細取得ViewModelAdapterの単体テスト
/// </summary>
[TestClass]
public class GetOrderDetailsViewModelAdapterTests
{
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private GetOrderDetailsViewModelAdapter _adapter = null!;

    /// <summary>
    /// テストの事前準備
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _productRepositoryMock =
            new Mock<IProductRepository>();

        _adapter =
            new GetOrderDetailsViewModelAdapter(
                _productRepositoryMock.Object);
    }

    /// <summary>
    /// 注文明細一覧をレスポンス一覧へ正しく変換できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-004 注文明細一覧をレスポンス一覧へ正しく変換する")]
    public async Task ConvertAsync_注文明細が存在する場合_レスポンス一覧へ変換する()
    {
        // Arrange
        const string firstProductUuid =
            "22222222-2222-2222-2222-222222222222";

        const string secondProductUuid =
            "33333333-3333-3333-3333-333333333333";

        var firstOrderDetail =
            OrderDetail.Restore(
                firstProductUuid,
                2);

        var secondOrderDetail =
            OrderDetail.Restore(
                secondProductUuid,
                1);

        var orderDetails =
            new List<OrderDetail>
            {
                firstOrderDetail,
                secondOrderDetail
            };

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectNameByProductUuidAsync(
                    firstProductUuid))
            .ReturnsAsync("ゲーミングマウス");

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(
                    firstProductUuid))
            .ReturnsAsync(3800);

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectNameByProductUuidAsync(
                    secondProductUuid))
            .ReturnsAsync("メカニカルキーボード");

        _productRepositoryMock
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(
                    secondProductUuid))
            .ReturnsAsync(8500);

        // Act
        var result =
            await _adapter.ConvertAsync(
                orderDetails);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);

        Assert.AreEqual(
            firstProductUuid,
            result[0].ProductUuid);

        Assert.AreEqual(
            "ゲーミングマウス",
            result[0].ProductName);

        Assert.AreEqual(
            3800,
            result[0].Price);

        Assert.AreEqual(
            2,
            result[0].Quantity);

        Assert.AreEqual(
            secondProductUuid,
            result[1].ProductUuid);

        Assert.AreEqual(
            "メカニカルキーボード",
            result[1].ProductName);

        Assert.AreEqual(
            8500,
            result[1].Price);

        Assert.AreEqual(
            1,
            result[1].Quantity);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectNameByProductUuidAsync(
                    firstProductUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectPriceByProductUuidAsync(
                    firstProductUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectNameByProductUuidAsync(
                    secondProductUuid),
            Times.Once);

        _productRepositoryMock.Verify(
            repository =>
                repository.SelectPriceByProductUuidAsync(
                    secondProductUuid),
            Times.Once);

        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 空の注文明細一覧を渡した場合、
    /// 空のレスポンス一覧が返されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-005 空の注文明細一覧を空のレスポンス一覧へ変換する")]
    public async Task ConvertAsync_注文明細が空の場合_空のレスポンス一覧を返す()
    {
        // Arrange
        var orderDetails =
            new List<OrderDetail>();

        // Act
        var result =
            await _adapter.ConvertAsync(
                orderDetails);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        _productRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 商品情報の取得中に例外が発生した場合、
    /// 例外が呼び出し元へ通知されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-006 商品情報の取得で例外が発生した場合、例外を通知する")]
    public async Task ConvertAsync_商品情報の取得で例外が発生した場合_例外を通知する()
    {
        // Arrange
        const string productUuid =
            "22222222-2222-2222-2222-222222222222";

        const string errorMessage =
            "商品情報の取得に失敗しました。";

        var orderDetails =
            new List<OrderDetail>
            {
                OrderDetail.Restore(
                    productUuid,
                    2)
            };

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
                    _adapter.ConvertAsync(
                        orderDetails));

        // Assert
        Assert.AreEqual(
            errorMessage,
            exception.Message);

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

        _productRepositoryMock.VerifyNoOtherCalls();
    }
}