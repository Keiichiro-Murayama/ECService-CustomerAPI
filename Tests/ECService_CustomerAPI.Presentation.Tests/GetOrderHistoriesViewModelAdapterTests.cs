using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Presentation.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Presentation.Tests.Adapters;

/// <summary>
/// 購入履歴取得ViewModelAdapterの単体テスト
/// </summary>
[TestClass]
public class GetOrderHistoriesViewModelAdapterTests
{
    private GetOrderHistoriesViewModelAdapter _adapter = null!;

    /// <summary>
    /// テストの事前準備
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        _adapter = new GetOrderHistoriesViewModelAdapter();
    }

    /// <summary>
    /// 注文一覧を購入履歴レスポンス一覧へ正しく変換できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-004 注文一覧を購入履歴レスポンス一覧へ正しく変換する")]
    public void ConvertAsync_注文一覧が存在する場合_購入履歴レスポンス一覧へ変換する()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440000";

        var firstOrderDate = new DateTimeOffset(
            2026,
            7,
            20,
            10,
            30,
            0,
            TimeSpan.Zero);

        var secondOrderDate = new DateTimeOffset(
            2026,
            7,
            10,
            15,
            45,
            0,
            TimeSpan.Zero);

        var firstOrder = Orders.Restore(
            1,
            "11111111-1111-1111-1111-111111111111",
            firstOrderDate,
            3000,
            customerUuid,
            1,
            1,
            new List<OrderDetail>());

        var secondOrder = Orders.Restore(
            2,
            "22222222-2222-2222-2222-222222222222",
            secondOrderDate,
            1500,
            customerUuid,
            1,
            2,
            new List<OrderDetail>());

        var orders = new List<Orders>
        {
            firstOrder,
            secondOrder
        };

        // Act
        var result = _adapter.ConvertAsync(orders);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);

        Assert.AreEqual(
            firstOrder.Id,
            result[0].OrderId);

        Assert.AreEqual(
            firstOrder.OrderUuid,
            result[0].OrderUuid);

        Assert.AreEqual(
            firstOrder.OrderDate,
            result[0].OrderDate);

        Assert.AreEqual(
            firstOrder.AmountTotal,
            result[0].AmountTotal);

        Assert.AreEqual(
            secondOrder.Id,
            result[1].OrderId);

        Assert.AreEqual(
            secondOrder.OrderUuid,
            result[1].OrderUuid);

        Assert.AreEqual(
            secondOrder.OrderDate,
            result[1].OrderDate);

        Assert.AreEqual(
            secondOrder.AmountTotal,
            result[1].AmountTotal);
    }

    /// <summary>
    /// 空の注文一覧を渡した場合、空のレスポンス一覧が返されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-HIS-005 空の注文一覧を空のレスポンス一覧へ変換する")]
    public void ConvertAsync_注文一覧が空の場合_空のレスポンス一覧を返す()
    {
        // Arrange
        var orders = new List<Orders>();

        // Act
        var result = _adapter.ConvertAsync(orders);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }
}