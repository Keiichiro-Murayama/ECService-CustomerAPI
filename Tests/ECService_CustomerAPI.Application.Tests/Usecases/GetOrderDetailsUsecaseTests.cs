using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService_CustomerAPI.Application.Tests.Usecases;

/// <summary>
/// 注文明細取得Usecaseの単体テスト
/// </summary>
[TestClass]
public class GetOrderDetailsUsecaseTests
{
    /// <summary>
    /// 注文明細が存在する場合、
    /// Repositoryから取得した注文明細一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-001 注文明細が存在する場合、注文明細一覧を返す")]
    public async Task ExecuteAsync_注文明細が存在する場合_注文明細一覧を返す()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        //石原:変更 他顧客の注文明細取得を防ぐため、顧客UUIDをテスト条件に追加
        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        var firstOrderDetail =
            OrderDetail.Restore(
                "22222222-2222-2222-2222-222222222222",
                2);

        var secondOrderDetail =
            OrderDetail.Restore(
                "33333333-3333-3333-3333-333333333333",
                1);

        var expectedOrderDetails =
            new List<OrderDetail>
            {
                firstOrderDetail,
                secondOrderDetail
            };

        var ordersRepositoryMock =
            new Mock<IOrdersRepository>();

        ordersRepositoryMock
            .Setup(repository =>
                repository.SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customerUuid))
            .ReturnsAsync(expectedOrderDetails);

        var usecase =
            new GetOrderDetailsUsecase(
                ordersRepositoryMock.Object);

        // Act
        var result =
            await usecase.ExecuteAsync(
                orderUuid,
                customerUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);

        Assert.AreEqual(
            firstOrderDetail.ProductUuid,
            result[0].ProductUuid);

        Assert.AreEqual(
            firstOrderDetail.Count,
            result[0].Count);

        Assert.AreEqual(
            secondOrderDetail.ProductUuid,
            result[1].ProductUuid);

        Assert.AreEqual(
            secondOrderDetail.Count,
            result[1].Count);

        ordersRepositoryMock.Verify(
            repository =>
                repository.SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        ordersRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 注文明細が存在しない場合、
    /// 空の一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-002 注文明細が存在しない場合、空の一覧を返す")]
    public async Task ExecuteAsync_注文明細が存在しない場合_空の一覧を返す()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        //石原:変更 他顧客の注文明細取得を防ぐため、顧客UUIDをテスト条件に追加
        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        var ordersRepositoryMock =
            new Mock<IOrdersRepository>();

        ordersRepositoryMock
            .Setup(repository =>
                repository.SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customerUuid))
            .ReturnsAsync(
                new List<OrderDetail>());

        var usecase =
            new GetOrderDetailsUsecase(
                ordersRepositoryMock.Object);

        // Act
        var result =
            await usecase.ExecuteAsync(
                orderUuid,
                customerUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        ordersRepositoryMock.Verify(
            repository =>
                repository.SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        ordersRepositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Repositoryで例外が発生した場合、
    /// 例外が呼び出し元へ通知されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-DET-003 Repositoryで例外が発生した場合、例外を通知する")]
    public async Task ExecuteAsync_Repositoryで例外が発生した場合_例外を通知する()
    {
        // Arrange
        const string orderUuid =
            "11111111-1111-1111-1111-111111111111";

        //石原:変更 他顧客の注文明細取得を防ぐため、顧客UUIDをテスト条件に追加
        const string customerUuid =
            "44444444-4444-4444-4444-444444444444";

        const string errorMessage =
            "注文明細の取得に失敗しました。";

        var ordersRepositoryMock =
            new Mock<IOrdersRepository>();

        ordersRepositoryMock
            .Setup(repository =>
                repository.SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customerUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    errorMessage));

        var usecase =
            new GetOrderDetailsUsecase(
                ordersRepositoryMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                () =>
                    usecase.ExecuteAsync(
                        orderUuid,
                        customerUuid));

        // Assert
        Assert.AreEqual(
            errorMessage,
            exception.Message);

        ordersRepositoryMock.Verify(
            repository =>
                repository.SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customerUuid),
            Times.Once);

        ordersRepositoryMock.VerifyNoOtherCalls();
    }
}