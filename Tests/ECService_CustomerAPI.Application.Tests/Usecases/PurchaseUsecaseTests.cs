using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.UnitOfWorks;
using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Moq;

namespace ECService_CustomerAPI.Application.Tests.Usecases;

/// <summary>
/// 商品購入ユースケースの単体テスト
/// Domain層のバリデーション詳細と、実DBを使用するRepositoryの動作は対象外。
/// </summary>
[TestClass]
public class PurchaseUsecaseTests
{
    private const string CustomerUuid =
        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

    private const string OrderUuid =
        "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";

    private const string PenUuid =
        "3fd7d44e-7cac-444b-b747-44eb988a0421";

    private const string NoteUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    private const string MissingProductUuid =
        "11111111-1111-1111-1111-111111111111";

    private Mock<ICustomerRepository> _customerRepository = null!;
    private Mock<IPaymentMethodRepository> _paymentMethodRepository = null!;
    private Mock<IProductRepository> _productRepository = null!;
    private Mock<IOrdersRepository> _ordersRepository = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private PurchaseUsecase _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _customerRepository =
            new Mock<ICustomerRepository>(MockBehavior.Strict);

        _paymentMethodRepository =
            new Mock<IPaymentMethodRepository>(MockBehavior.Strict);

        _productRepository =
            new Mock<IProductRepository>(MockBehavior.Strict);

        _ordersRepository =
            new Mock<IOrdersRepository>(MockBehavior.Strict);

        _unitOfWork =
            new Mock<IUnitOfWork>(MockBehavior.Strict);

        _sut = new PurchaseUsecase(
            _customerRepository.Object,
            _paymentMethodRepository.Object,
            _productRepository.Object,
            _ordersRepository.Object,
            _unitOfWork.Object);
    }

    /// <summary>
    /// PUR-APP-001
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_複数商品を購入した場合_合計金額が正しく計算される()
    {
        // Arrange
        SetupExistingCustomerAndPaymentMethod();

        _unitOfWork
            .Setup(unit => unit.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _productRepository
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(PenUuid))
            .ReturnsAsync(1200);

        _productRepository
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(NoteUuid))
            .ReturnsAsync(450);

        _productRepository
            .Setup(repository =>
                repository.UpdateProductStockAsync(PenUuid, 2))
            .Returns(Task.CompletedTask);

        _productRepository
            .Setup(repository =>
                repository.UpdateProductStockAsync(NoteUuid, 3))
            .Returns(Task.CompletedTask);

        Orders? createdOrder = null;

        _ordersRepository
            .Setup(repository =>
                repository.CreateAsync(It.IsAny<Orders>()))
            .Callback<Orders>(order => createdOrder = order)
            .ReturnsAsync(OrderUuid);

        _unitOfWork
            .Setup(unit => unit.CommitAsync())
            .Returns(Task.CompletedTask);

        var items =
            new List<(string ProductUuid, int Quantity)>
            {
                (PenUuid, 2),
                (NoteUuid, 3)
            };

        // Act
        var actualOrderUuid =
            await _sut.ExecuteAsync(
                CustomerUuid,
                1,
                items);

        // Assert
        Assert.AreEqual(OrderUuid, actualOrderUuid);
        Assert.IsNotNull(createdOrder);

        Assert.AreEqual(3750, createdOrder!.AmountTotal);
        Assert.AreEqual(CustomerUuid, createdOrder.CustomerUuid);
        Assert.AreEqual(1, createdOrder.PaymentMethodId);
        Assert.AreEqual(1, createdOrder.OrderStatusId);
        Assert.HasCount(2, createdOrder.OrderDetails);

        Assert.AreEqual(
            PenUuid,
            createdOrder.OrderDetails[0].ProductUuid);

        Assert.AreEqual(
            2,
            createdOrder.OrderDetails[0].Count);

        Assert.AreEqual(
            NoteUuid,
            createdOrder.OrderDetails[1].ProductUuid);

        Assert.AreEqual(
            3,
            createdOrder.OrderDetails[1].Count);

        _unitOfWork.Verify(
            unit => unit.BeginTransactionAsync(),
            Times.Once);

        _unitOfWork.Verify(
            unit => unit.CommitAsync(),
            Times.Once);

        _unitOfWork.Verify(
            unit => unit.RollbackAsync(),
            Times.Never);

        _productRepository.Verify(
            repository =>
                repository.UpdateProductStockAsync(PenUuid, 2),
            Times.Once);

        _productRepository.Verify(
            repository =>
                repository.UpdateProductStockAsync(NoteUuid, 3),
            Times.Once);

        _ordersRepository.Verify(
            repository =>
                repository.CreateAsync(It.IsAny<Orders>()),
            Times.Once);
    }

    /// <summary>
    /// PUR-APP-002
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_購入商品が指定されていない場合_DomainExceptionが発生する()
    {
        // Arrange
        var testCases =
            new List<(string ProductUuid, int Quantity)>?[]
            {
                null,
                new List<(string ProductUuid, int Quantity)>()
            };

        foreach (var items in testCases)
        {
            // Act
            var exception =
                await Assert.ThrowsAsync<DomainException>(
                    () => _sut.ExecuteAsync(
                        CustomerUuid,
                        1,
                        items!));

            // Assert
            Assert.AreEqual(
                "購入する商品を1件以上指定してください。",
                exception.Message);
        }

        _customerRepository.Verify(
            repository =>
                repository.FindByUuidAsync(It.IsAny<string>()),
            Times.Never);

        _unitOfWork.Verify(
            unit => unit.BeginTransactionAsync(),
            Times.Never);
    }

    /// <summary>
    /// PUR-APP-003
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_顧客が存在しない場合_NotFoundExceptionが発生する()
    {
        // Arrange
        _customerRepository
            .Setup(repository =>
                repository.FindByUuidAsync(CustomerUuid))
            .ReturnsAsync((Customer?)null);

        var items =
            new List<(string ProductUuid, int Quantity)>
            {
                (PenUuid, 1)
            };

        // Act
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.ExecuteAsync(
                    CustomerUuid,
                    1,
                    items));

        // Assert
        Assert.AreEqual(
            "顧客が見つかりません。",
            exception.Message);

        _paymentMethodRepository.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        _unitOfWork.Verify(
            unit => unit.BeginTransactionAsync(),
            Times.Never);
    }

    /// <summary>
    /// PUR-APP-004
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_支払い方法が存在しない場合_NotFoundExceptionが発生する()
    {
        // Arrange
        _customerRepository
            .Setup(repository =>
                repository.FindByUuidAsync(CustomerUuid))
            .ReturnsAsync(CreateCustomer());

        _paymentMethodRepository
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(
                new List<PaymentMethod>
                {
                    PaymentMethod.Restore(1, "現金"),
                    PaymentMethod.Restore(2, "カード")
                });

        var items =
            new List<(string ProductUuid, int Quantity)>
            {
                (PenUuid, 1)
            };

        // Act
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.ExecuteAsync(
                    CustomerUuid,
                    99,
                    items));

        // Assert
        Assert.AreEqual(
            "指定されたリソースが見つかりません。",
            exception.Message);

        _unitOfWork.Verify(
            unit => unit.BeginTransactionAsync(),
            Times.Never);

        _productRepository.Verify(
            repository =>
                repository.SelectPriceByProductUuidAsync(
                    It.IsAny<string>()),
            Times.Never);

        _ordersRepository.Verify(
            repository =>
                repository.CreateAsync(It.IsAny<Orders>()),
            Times.Never);
    }

    /// <summary>
    /// PUR-APP-005
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品が存在しない場合_NotFoundExceptionが発生してロールバックされる()
    {
        // Arrange
        SetupExistingCustomerAndPaymentMethod();

        _unitOfWork
            .Setup(unit => unit.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _productRepository
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(
                    MissingProductUuid))
            .ThrowsAsync(
                new InternalException(
                    "指定された商品が見つかりません。"));

        _unitOfWork
            .Setup(unit => unit.RollbackAsync())
            .Returns(Task.CompletedTask);

        var items =
            new List<(string ProductUuid, int Quantity)>
            {
                (MissingProductUuid, 1)
            };

        // Act
        var exception =
            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.ExecuteAsync(
                    CustomerUuid,
                    1,
                    items));

        // Assert
        Assert.IsTrue(
            exception.Message.Contains(
                MissingProductUuid,
                StringComparison.Ordinal));

        _unitOfWork.Verify(
            unit => unit.RollbackAsync(),
            Times.Once);

        _unitOfWork.Verify(
            unit => unit.CommitAsync(),
            Times.Never);

        _productRepository.Verify(
            repository =>
                repository.UpdateProductStockAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>()),
            Times.Never);

        _ordersRepository.Verify(
            repository =>
                repository.CreateAsync(It.IsAny<Orders>()),
            Times.Never);
    }

    /// <summary>
    /// PUR-APP-006
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_在庫不足の場合_DomainExceptionが発生してロールバックされる()
    {
        // Arrange
        SetupExistingCustomerAndPaymentMethod();

        _unitOfWork
            .Setup(unit => unit.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _productRepository
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(PenUuid))
            .ReturnsAsync(1200);

        const string expectedMessage =
            "申し訳ありませんが、商品「高級ボールペン」の在庫が不足しています。";

        _productRepository
            .Setup(repository =>
                repository.UpdateProductStockAsync(PenUuid, 999))
            .ThrowsAsync(
                new DomainException(
                    expectedMessage,
                    "subtractedQuantity"));

        _unitOfWork
            .Setup(unit => unit.RollbackAsync())
            .Returns(Task.CompletedTask);

        var items =
            new List<(string ProductUuid, int Quantity)>
            {
                (PenUuid, 999)
            };

        // Act
        var exception =
            await Assert.ThrowsAsync<DomainException>(
                () => _sut.ExecuteAsync(
                    CustomerUuid,
                    1,
                    items));

        // Assert
        Assert.AreEqual(
            expectedMessage,
            exception.Message);

        _unitOfWork.Verify(
            unit => unit.RollbackAsync(),
            Times.Once);

        _unitOfWork.Verify(
            unit => unit.CommitAsync(),
            Times.Never);

        _ordersRepository.Verify(
            repository =>
                repository.CreateAsync(It.IsAny<Orders>()),
            Times.Never);
    }

    /// <summary>
    /// PUR-APP-007
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_注文登録で内部エラーが発生した場合_ロールバックされる()
    {
        // Arrange
        SetupExistingCustomerAndPaymentMethod();

        _unitOfWork
            .Setup(unit => unit.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _productRepository
            .Setup(repository =>
                repository.SelectPriceByProductUuidAsync(PenUuid))
            .ReturnsAsync(1200);

        _productRepository
            .Setup(repository =>
                repository.UpdateProductStockAsync(PenUuid, 2))
            .Returns(Task.CompletedTask);

        _ordersRepository
            .Setup(repository =>
                repository.CreateAsync(It.IsAny<Orders>()))
            .ThrowsAsync(
                new InternalException(
                    "注文登録処理でエラーが発生しました。"));

        _unitOfWork
            .Setup(unit => unit.RollbackAsync())
            .Returns(Task.CompletedTask);

        var items =
            new List<(string ProductUuid, int Quantity)>
            {
                (PenUuid, 2)
            };

        // Act
        var exception =
            await Assert.ThrowsAsync<InternalException>(
                () => _sut.ExecuteAsync(
                    CustomerUuid,
                    1,
                    items));

        // Assert
        Assert.AreEqual(
            "注文登録処理でエラーが発生しました。",
            exception.Message);

        _unitOfWork.Verify(
            unit => unit.RollbackAsync(),
            Times.Once);

        _unitOfWork.Verify(
            unit => unit.CommitAsync(),
            Times.Never);
    }

    private void SetupExistingCustomerAndPaymentMethod()
    {
        _customerRepository
            .Setup(repository =>
                repository.FindByUuidAsync(CustomerUuid))
            .ReturnsAsync(CreateCustomer());

        _paymentMethodRepository
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(
                new List<PaymentMethod>
                {
                    PaymentMethod.Restore(1, "現金")
                });
    }

    private static Customer CreateCustomer()
    {
        return Customer.Restore(
            CustomerUuid,
            "テスト顧客",
            "テストコキャク",
            "東京都千代田区1-1",
            string.Empty,
            "090-1234-5678",
            "test@example.com",
            "testuser",
            "hashed-password");
    }
}