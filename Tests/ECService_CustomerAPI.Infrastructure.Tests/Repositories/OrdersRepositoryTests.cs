using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Infrastructure.Adapters;
using ECService_CustomerAPI.Infrastructure.Contexts;
using ECService_CustomerAPI.Infrastructure.Entities;
using ECService_CustomerAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Repositories;

/// <summary>
/// OrdersRepositoryの単体テスト
/// </summary>
[TestClass]
[DoNotParallelize]
public class OrdersRepositoryTests
{
    private AppDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;
    private OrdersRepository _repository = null!;

    /// <summary>
    /// テストごとにDB接続とトランザクションを開始する
    /// </summary>
    [TestInitialize]
    public async Task InitializeAsync()
    {
        var configuration =
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(
                    "Repositories/appsettingsTests.json",
                    optional: false)
                .Build();

        var connectionString =
            configuration.GetConnectionString(
                "ECServiceDB")
            ?? throw new InvalidOperationException(
                "テストDBの接続文字列を取得できませんでした。");

        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        _context =
            new AppDbContext(options);

        _transaction =
            await _context.Database
                .BeginTransactionAsync();

        _repository =
            new OrdersRepository(
                _context,
                new OrdersEntityAdapter(),
                new OrderDetailEntityAdapter());
    }

    /// <summary>
    /// テストによるDB変更をロールバックする
    /// </summary>
    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        await _context.DisposeAsync();
    }

    /// <summary>
    /// 注文と注文明細を正常に登録できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-001 有効な注文を渡した場合、注文と注文明細を登録する")]
    public async Task CreateAsync_有効な注文を渡した場合_注文と注文明細を登録する()
    {
        // Arrange
        var testData =
            await AddPrerequisitesAsync();

        var orderUuid =
            Guid.NewGuid().ToString();

        var orderDetails =
            new List<OrderDetail>
            {
                OrderDetail.Restore(
                    testData.FirstProduct
                        .ProductUuid
                        .ToString(),
                    2),

                OrderDetail.Restore(
                    testData.SecondProduct
                        .ProductUuid
                        .ToString(),
                    3)
            };

        var order =
            Orders.Restore(
                0,
                orderUuid,
                DateTimeOffset.UtcNow.AddDays(-1),
                9800,
                testData.Customer
                    .CustomerUuid
                    .ToString(),
                testData.OrderStatus.Id,
                testData.PaymentMethod.Id,
                orderDetails);

        var beforeExecution =
            DateTimeOffset.UtcNow;

        // Act
        var result =
            await _repository.CreateAsync(
                order);

        var afterExecution =
            DateTimeOffset.UtcNow;

        // Assert
        Assert.AreEqual(
            orderUuid,
            result);

        var savedOrder =
            await _context.Orders
                .AsNoTracking()
                .Include(entity =>
                    entity.OrdersDetails)
                .SingleAsync(entity =>
                    entity.OrderUuid ==
                    Guid.Parse(orderUuid));

        Assert.AreEqual(
            testData.Customer.Id,
            savedOrder.CustomerId);

        Assert.AreEqual(
            testData.OrderStatus.Id,
            savedOrder.OrderStatusId);

        Assert.AreEqual(
            testData.PaymentMethod.Id,
            savedOrder.PaymentMethodId);

        Assert.AreEqual(
            9800,
            savedOrder.AmountTotal);

        Assert.IsNotNull(
            savedOrder.OrderDate);

        Assert.IsTrue(
            savedOrder.OrderDate >=
            beforeExecution);

        Assert.IsTrue(
            savedOrder.OrderDate <=
            afterExecution);

        Assert.HasCount(
            2,
            savedOrder.OrdersDetails);

        var firstDetail =
            savedOrder.OrdersDetails
                .Single(detail =>
                    detail.ProductId ==
                    testData.FirstProduct.Id);

        Assert.AreEqual(
            2,
            firstDetail.Count);

        var secondDetail =
            savedOrder.OrdersDetails
                .Single(detail =>
                    detail.ProductId ==
                    testData.SecondProduct.Id);

        Assert.AreEqual(
            3,
            secondDetail.Count);
    }

    /// <summary>
    /// 顧客UUIDが存在しない場合に例外となること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-002 顧客UUIDが存在しない場合、InternalExceptionを通知する")]
    public async Task CreateAsync_顧客が存在しない場合_InternalExceptionを通知する()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid().ToString();

        var productUuid =
            Guid.NewGuid().ToString();

        var order =
            Orders.Restore(
                0,
                Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow,
                1000,
                customerUuid,
                1,
                1,
                new List<OrderDetail>
                {
                    OrderDetail.Restore(
                        productUuid,
                        1)
                });

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    _repository.CreateAsync(
                        order));

        // Assert
        StringAssert.Contains(
            exception.Message,
            customerUuid);

        Assert.IsFalse(
            await _context.Orders
                .AnyAsync(entity =>
                    entity.OrderUuid ==
                    Guid.Parse(order.OrderUuid)));
    }

    /// <summary>
    /// 商品UUIDが存在しない場合に例外となること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-003 商品UUIDが存在しない場合、InternalExceptionを通知する")]
    public async Task CreateAsync_商品が存在しない場合_InternalExceptionを通知する()
    {
        // Arrange
        var customer =
            await AddCustomerAsync();

        var productUuid =
            Guid.NewGuid().ToString();

        var order =
            Orders.Restore(
                0,
                Guid.NewGuid().ToString(),
                DateTimeOffset.UtcNow,
                1000,
                customer.CustomerUuid.ToString(),
                1,
                1,
                new List<OrderDetail>
                {
                    OrderDetail.Restore(
                        productUuid,
                        1)
                });

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    _repository.CreateAsync(
                        order));

        // Assert
        StringAssert.Contains(
            exception.Message,
            productUuid);

        Assert.IsFalse(
            await _context.Orders
                .AnyAsync(entity =>
                    entity.OrderUuid ==
                    Guid.Parse(order.OrderUuid)));
    }

    /// <summary>
    /// 指定した顧客の注文だけを取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-004 顧客UUIDを指定した場合、対象顧客の注文だけを取得する")]
    public async Task SelectByCustomerUuidAsync_注文が存在する場合_対象顧客の注文だけを返す()
    {
        // Arrange
        var testData =
            await AddPrerequisitesAsync();

        var otherCustomer =
            await AddCustomerAsync();

        var firstOrder =
            await AddOrderAsync(
                testData.Customer,
                testData.OrderStatus,
                testData.PaymentMethod,
                testData.FirstProduct,
                2000,
                2);

        var secondOrder =
            await AddOrderAsync(
                testData.Customer,
                testData.OrderStatus,
                testData.PaymentMethod,
                testData.SecondProduct,
                3000,
                1);

        await AddOrderAsync(
            otherCustomer,
            testData.OrderStatus,
            testData.PaymentMethod,
            testData.FirstProduct,
            5000,
            4);

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository
                .SelectByCustomerUuidAsync(
                    testData.Customer
                        .CustomerUuid
                        .ToString());

        // Assert
        Assert.HasCount(
            2,
            result);

        Assert.IsTrue(
            result.Any(order =>
                order.OrderUuid ==
                firstOrder.OrderUuid.ToString()));

        Assert.IsTrue(
            result.Any(order =>
                order.OrderUuid ==
                secondOrder.OrderUuid.ToString()));

        Assert.IsTrue(
            result.All(order =>
                order.CustomerUuid ==
                testData.Customer
                    .CustomerUuid
                    .ToString()));

        var firstResult =
            result.Single(order =>
                order.OrderUuid ==
                firstOrder.OrderUuid.ToString());

        Assert.AreEqual(
            firstOrder.AmountTotal,
            firstResult.AmountTotal);

        Assert.HasCount(
            1,
            firstResult.OrderDetails);

        Assert.AreEqual(
            testData.FirstProduct
                .ProductUuid
                .ToString(),
            firstResult.OrderDetails[0]
                .ProductUuid);

        Assert.AreEqual(
            2,
            firstResult.OrderDetails[0]
                .Count);
    }

    /// <summary>
    /// 注文が存在しない場合に空の一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-005 顧客に注文が存在しない場合、空の一覧を返す")]
    public async Task SelectByCustomerUuidAsync_注文が存在しない場合_空の一覧を返す()
    {
        // Arrange
        var customer =
            await AddCustomerAsync();

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository
                .SelectByCustomerUuidAsync(
                    customer.CustomerUuid.ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 本人の注文に含まれる注文明細を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-006 本人の注文UUIDを指定した場合、注文明細を取得する")]
    public async Task SelectOrderDetailsByOrderUuidAsync_本人の注文の場合_注文明細を返す()
    {
        // Arrange
        var testData =
            await AddPrerequisitesAsync();

        var order =
            await AddOrderAsync(
                testData.Customer,
                testData.OrderStatus,
                testData.PaymentMethod,
                testData.FirstProduct,
                4000,
                2);

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository
                .SelectOrderDetailsByOrderUuidAsync(
                    order.OrderUuid.ToString(),
                    testData.Customer
                        .CustomerUuid
                        .ToString());

        // Assert
        Assert.HasCount(
            1,
            result);

        Assert.AreEqual(
            testData.FirstProduct
                .ProductUuid
                .ToString(),
            result[0].ProductUuid);

        Assert.AreEqual(
            2,
            result[0].Count);
    }

    /// <summary>
    /// 注文明細がない注文の場合に空の一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-007 注文に明細が存在しない場合、空の一覧を返す")]
    public async Task SelectOrderDetailsByOrderUuidAsync_明細が存在しない場合_空の一覧を返す()
    {
        // Arrange
        var testData =
            await AddPrerequisitesAsync();

        var order =
            new OrdersEntity
            {
                OrderUuid =
                    Guid.NewGuid(),

                OrderDate =
                    DateTimeOffset.UtcNow,

                AmountTotal =
                    0,

                CustomerId =
                    testData.Customer.Id,

                OrderStatusId =
                    testData.OrderStatus.Id,

                PaymentMethodId =
                    testData.PaymentMethod.Id
            };

        _context.Orders.Add(
            order);

        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository
                .SelectOrderDetailsByOrderUuidAsync(
                    order.OrderUuid.ToString(),
                    testData.Customer
                        .CustomerUuid
                        .ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 存在しない注文UUIDの場合に空の一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-008 注文UUIDが存在しない場合、空の一覧を返す")]
    public async Task SelectOrderDetailsByOrderUuidAsync_注文が存在しない場合_空の一覧を返す()
    {
        // Arrange
        var customer =
            await AddCustomerAsync();

        var orderUuid =
            Guid.NewGuid().ToString();

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository
                .SelectOrderDetailsByOrderUuidAsync(
                    orderUuid,
                    customer.CustomerUuid.ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 他顧客の注文の場合に空の一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-ORD-009 他顧客の注文UUIDを指定した場合、空の一覧を返す")]
    public async Task SelectOrderDetailsByOrderUuidAsync_他顧客の注文の場合_空の一覧を返す()
    {
        // Arrange
        var testData =
            await AddPrerequisitesAsync();

        var otherCustomer =
            await AddCustomerAsync();

        var otherCustomerOrder =
            await AddOrderAsync(
                otherCustomer,
                testData.OrderStatus,
                testData.PaymentMethod,
                testData.FirstProduct,
                3000,
                1);

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository
                .SelectOrderDetailsByOrderUuidAsync(
                    otherCustomerOrder
                        .OrderUuid
                        .ToString(),
                    testData.Customer
                        .CustomerUuid
                        .ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 注文登録に必要な関連データを作成する
    /// </summary>
    private async Task<OrderTestData>
        AddPrerequisitesAsync()
    {
        var token =
            CreateToken();

        var customer =
            CreateCustomerEntity();

        var category =
            new ProductCategoryEntity
            {
                CategoryUuid =
                    Guid.NewGuid(),

                Name =
                    $"カテゴリ_{token}"
            };

        var orderStatus =
            new OrderStatusEntity
            {
                Name =
                    $"注文状態_{token}"
            };

        var paymentMethod =
            new PaymentMethodEntity
            {
                Name =
                    $"支払方法_{token}"
            };

        _context.AddRange(
            customer,
            category,
            orderStatus,
            paymentMethod);

        await _context.SaveChangesAsync();

        var firstProduct =
            new ProductEntity
            {
                ProductUuid =
                    Guid.NewGuid(),

                ProductCategoryId =
                    category.Id,

                Name =
                    $"商品A_{token}",

                Price =
                    2000,

                ImageUrl =
                    null,

                DeleteFlag =
                    0
            };

        var secondProduct =
            new ProductEntity
            {
                ProductUuid =
                    Guid.NewGuid(),

                ProductCategoryId =
                    category.Id,

                Name =
                    $"商品B_{token}",

                Price =
                    3000,

                ImageUrl =
                    null,

                DeleteFlag =
                    0
            };

        _context.Products.AddRange(
            firstProduct,
            secondProduct);

        await _context.SaveChangesAsync();

        return new OrderTestData(
            customer,
            orderStatus,
            paymentMethod,
            firstProduct,
            secondProduct);
    }

    /// <summary>
    /// テスト用顧客をDBへ登録する
    /// </summary>
    private async Task<CustomerEntity>
        AddCustomerAsync()
    {
        var customer =
            CreateCustomerEntity();

        _context.Customers.Add(
            customer);

        await _context.SaveChangesAsync();

        return customer;
    }

    /// <summary>
    /// テスト用注文をDBへ登録する
    /// </summary>
    private async Task<OrdersEntity>
        AddOrderAsync(
            CustomerEntity customer,
            OrderStatusEntity orderStatus,
            PaymentMethodEntity paymentMethod,
            ProductEntity product,
            int amountTotal,
            int count)
    {
        var order =
            new OrdersEntity
            {
                OrderUuid =
                    Guid.NewGuid(),

                OrderDate =
                    DateTimeOffset.UtcNow,

                AmountTotal =
                    amountTotal,

                CustomerId =
                    customer.Id,

                OrderStatusId =
                    orderStatus.Id,

                PaymentMethodId =
                    paymentMethod.Id,

                OrdersDetails =
                    new List<OrderDetailEntity>
                    {
                        new()
                        {
                            ProductId =
                                product.Id,

                            Count =
                                count
                        }
                    }
            };

        _context.Orders.Add(
            order);

        await _context.SaveChangesAsync();

        return order;
    }

    /// <summary>
    /// テスト用顧客エンティティを生成する
    /// </summary>
    private static CustomerEntity
        CreateCustomerEntity()
    {
        var token =
            CreateToken();

        return new CustomerEntity
        {
            CustomerUuid =
                Guid.NewGuid(),

            Name =
                "テスト太郎",

            NameKana =
                "テストタロウ",

            Address1 =
                "愛知県名古屋市",

            Address2 =
                "テストマンション",

            PhoneNumber =
                CreatePhoneNumber(),

            MailAddress =
                $"{token}@example.com",

            Username =
                $"user{token}",

            Password =
                "hashed-password",

            CreatedAt =
                DateTime.UtcNow
        };
    }

    /// <summary>
    /// 一意な文字列を生成する
    /// </summary>
    private static string CreateToken()
    {
        return Guid.NewGuid()
            .ToString("N")[..8];
    }

    /// <summary>
    /// 一意な電話番号を生成する
    /// </summary>
    private static string CreatePhoneNumber()
    {
        var number =
            Random.Shared
                .Next(
                    0,
                    100_000_000)
                .ToString("D8");

        return
            $"090-{number[..4]}-{number[4..]}";
    }

    /// <summary>
    /// 注文テスト用関連データ
    /// </summary>
    private sealed record OrderTestData(
        CustomerEntity Customer,
        OrderStatusEntity OrderStatus,
        PaymentMethodEntity PaymentMethod,
        ProductEntity FirstProduct,
        ProductEntity SecondProduct);
}