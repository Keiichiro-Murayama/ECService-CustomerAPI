using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Application.Tests;

/// <summary>
/// 購入履歴取得Usecaseの単体テスト
/// </summary>
[TestClass]
public class GetOrderHistoriesUsecaseTests
{
    /// <summary>
    /// 購入履歴が存在する場合、購入日時の降順で返されること
    /// </summary>
    [TestMethod(DisplayName = "UT-HIS-001 購入履歴が存在する場合、購入日時の降順で返す")]
    public async Task ExecuteAsync_購入履歴が存在する場合_購入日時の降順で返す()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440000";

        var oldestOrder = Orders.Restore(
            1,
            "11111111-1111-1111-1111-111111111111",
            new DateTimeOffset(
                2026,
                7,
                1,
                10,
                0,
                0,
                TimeSpan.Zero),
            1000,
            customerUuid,
            1,
            1,
            new List<OrderDetail>());

        var newestOrder = Orders.Restore(
            2,
            "22222222-2222-2222-2222-222222222222",
            new DateTimeOffset(
                2026,
                7,
                20,
                10,
                0,
                0,
                TimeSpan.Zero),
            3000,
            customerUuid,
            1,
            1,
            new List<OrderDetail>());

        var middleOrder = Orders.Restore(
            3,
            "33333333-3333-3333-3333-333333333333",
            new DateTimeOffset(
                2026,
                7,
                10,
                10,
                0,
                0,
                TimeSpan.Zero),
            2000,
            customerUuid,
            1,
            1,
            new List<OrderDetail>());

        var ordersRepositoryMock =
            new Mock<IOrdersRepository>();

        ordersRepositoryMock
            .Setup(repository =>
                repository.SelectByCustomerUuidAsync(
                    customerUuid))
            .ReturnsAsync(
                new List<Orders>
                {
                    oldestOrder,
                    newestOrder,
                    middleOrder
                });

        var usecase =
            new GetOrderHistoriesUsecase(
                ordersRepositoryMock.Object);

        // Act
        var result =
            await usecase.ExecuteAsync(
                customerUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(3, result);

        Assert.AreEqual(
            newestOrder.OrderUuid,
            result[0].OrderUuid);

        Assert.AreEqual(
            middleOrder.OrderUuid,
            result[1].OrderUuid);

        Assert.AreEqual(
            oldestOrder.OrderUuid,
            result[2].OrderUuid);

        ordersRepositoryMock.Verify(
            repository =>
                repository.SelectByCustomerUuidAsync(
                    customerUuid),
            Times.Once);
    }

    /// <summary>
    /// 購入履歴が存在しない場合、空の一覧が返されること
    /// </summary>
    [TestMethod(DisplayName = "UT-HIS-002 購入履歴が存在しない場合、空の一覧を返す")]
    public async Task ExecuteAsync_購入履歴が存在しない場合_空の一覧を返す()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440099";

        var ordersRepositoryMock =
            new Mock<IOrdersRepository>();

        ordersRepositoryMock
            .Setup(repository =>
                repository.SelectByCustomerUuidAsync(
                    customerUuid))
            .ReturnsAsync(
                new List<Orders>());

        var usecase =
            new GetOrderHistoriesUsecase(
                ordersRepositoryMock.Object);

        // Act
        var result =
            await usecase.ExecuteAsync(
                customerUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        ordersRepositoryMock.Verify(
            repository =>
                repository.SelectByCustomerUuidAsync(
                    customerUuid),
            Times.Once);
    }

    /// <summary>
    /// Repositoryで例外が発生した場合、
    /// 例外が呼び出し元へ通知されること
    /// </summary>
    [TestMethod(DisplayName = "UT-HIS-003 Repositoryで例外が発生した場合、例外を通知する")]
    public async Task ExecuteAsync_Repositoryで例外が発生した場合_例外を通知する()
    {
        // Arrange
        const string customerUuid =
            "550e8400-e29b-41d4-a716-446655440000";

        const string errorMessage =
            "購入履歴の取得に失敗しました。";

        var ordersRepositoryMock =
            new Mock<IOrdersRepository>();

        ordersRepositoryMock
            .Setup(repository =>
                repository.SelectByCustomerUuidAsync(
                    customerUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    errorMessage));

        var usecase =
            new GetOrderHistoriesUsecase(
                ordersRepositoryMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                () => usecase.ExecuteAsync(
                    customerUuid));

        // Assert
        Assert.AreEqual(
            errorMessage,
            exception.Message);

        ordersRepositoryMock.Verify(
            repository =>
                repository.SelectByCustomerUuidAsync(
                    customerUuid),
            Times.Once);
    }
}