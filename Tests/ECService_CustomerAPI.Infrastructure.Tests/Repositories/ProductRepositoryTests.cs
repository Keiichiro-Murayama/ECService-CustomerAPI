using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Infrastructure.Contexts;
using ECService_CustomerAPI.Infrastructure.Entities;
using ECService_CustomerAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Repositories;

/// <summary>
/// ProductRepositoryの単体テスト
/// </summary>
[TestClass]
[DoNotParallelize]
public class ProductRepositoryTests
{
    private AppDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;
    private ProductRepository _repository = null!;


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
                optional: false,
                reloadOnChange: false)
            .AddEnvironmentVariables()
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
            new ProductRepository(
                _context);
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
    /// 登録済み商品の価格を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-001 登録済み商品UUIDの場合、価格を返す")]
    public async Task SelectPriceByProductUuidAsync_商品が存在する場合_価格を返す()
    {
        // Arrange
        var product =
            await AddProductAsync(
                price: 3800,
                quantity: 10);

        // Act
        var result =
            await _repository
                .SelectPriceByProductUuidAsync(
                    product.ProductUuid.ToString());

        // Assert
        Assert.AreEqual(
            product.Price,
            result);
    }

    /// <summary>
    /// 商品が存在しない場合に例外を通知すること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-002 未登録商品UUIDで価格を取得した場合、InternalExceptionを通知する")]
    public async Task SelectPriceByProductUuidAsync_商品が存在しない場合_InternalExceptionを通知する()
    {
        // Arrange
        var productUuid =
            Guid.NewGuid().ToString();

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    _repository
                        .SelectPriceByProductUuidAsync(
                            productUuid));

        // Assert
        Assert.AreEqual(
            $"商品UUID '{productUuid}' が見つかりません。",
            exception.Message);
    }

    /// <summary>
    /// 登録済み商品の在庫数を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-003 登録済み商品UUIDの場合、在庫数を返す")]
    public async Task SelectStockByProductUuidAsync_商品が存在する場合_在庫数を返す()
    {
        // Arrange
        var product =
            await AddProductAsync(
                price: 3800,
                quantity: 10);

        // Act
        var result =
            await _repository
                .SelectStockByProductUuidAsync(
                    product.ProductUuid.ToString());

        // Assert
        Assert.AreEqual(
            10,
            result);
    }

    /// <summary>
    /// 商品が存在しない場合に例外を通知すること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-004 未登録商品UUIDで在庫を取得した場合、InternalExceptionを通知する")]
    public async Task SelectStockByProductUuidAsync_商品が存在しない場合_InternalExceptionを通知する()
    {
        // Arrange
        var productUuid =
            Guid.NewGuid().ToString();

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    _repository
                        .SelectStockByProductUuidAsync(
                            productUuid));

        // Assert
        Assert.AreEqual(
            $"商品UUID '{productUuid}' が見つかりません。",
            exception.Message);
    }

    /// <summary>
    /// 商品在庫を購入数量分だけ減算できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-005 在庫が十分な場合、購入数量分だけ在庫を減算する")]
    public async Task UpdateProductStockAsync_在庫が十分な場合_在庫を減算する()
    {
        // Arrange
        var product =
            await AddProductAsync(
                price: 3800,
                quantity: 10);

        // Act
        await _repository
            .UpdateProductStockAsync(
                product.ProductUuid.ToString(),
                3);

        _context.ChangeTracker.Clear();

        // Assert
        var savedStock =
            await _context.ProductStocks
                .AsNoTracking()
                .SingleAsync(stock =>
                    stock.ProductId ==
                    product.Id);

        Assert.AreEqual(
            7,
            savedStock.Quantity);
    }

    /// <summary>
    /// 商品が存在しない場合に例外を通知すること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-006 未登録商品UUIDの在庫を更新した場合、InternalExceptionを通知する")]
    public async Task UpdateProductStockAsync_商品が存在しない場合_InternalExceptionを通知する()
    {
        // Arrange
        var productUuid =
            Guid.NewGuid().ToString();

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    _repository
                        .UpdateProductStockAsync(
                            productUuid,
                            1));

        // Assert
        Assert.AreEqual(
            $"商品UUID '{productUuid}' が見つかりません。",
            exception.Message);
    }

    /// <summary>
    /// 在庫が不足している場合に例外を通知し、
    /// 在庫数を変更しないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-007 購入数量が在庫数を超える場合、DomainExceptionを通知する")]
    public async Task UpdateProductStockAsync_在庫が不足している場合_DomainExceptionを通知する()
    {
        // Arrange
        var product =
            await AddProductAsync(
                price: 3800,
                quantity: 3);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                DomainException>(
                () =>
                    _repository
                        .UpdateProductStockAsync(
                            product.ProductUuid.ToString(),
                            4));

        _context.ChangeTracker.Clear();

        // Assert
        Assert.AreEqual(
            $"申し訳ありませんが、商品「{product.Name}」の在庫が不足しています。",
            exception.Message);

        var savedStock =
            await _context.ProductStocks
                .AsNoTracking()
                .SingleAsync(stock =>
                    stock.ProductId ==
                    product.Id);

        Assert.AreEqual(
            3,
            savedStock.Quantity);
    }

    /// <summary>
    /// 登録済み商品の商品名を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-008 登録済み商品UUIDの場合、商品名を返す")]
    public async Task SelectNameByProductUuidAsync_商品が存在する場合_商品名を返す()
    {
        // Arrange
        var product =
            await AddProductAsync(
                price: 3800,
                quantity: 10);

        // Act
        var result =
            await _repository
                .SelectNameByProductUuidAsync(
                    product.ProductUuid.ToString());

        // Assert
        Assert.AreEqual(
            product.Name,
            result);
    }

    /// <summary>
    /// 商品が存在しない場合に例外を通知すること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PRO-009 未登録商品UUIDで商品名を取得した場合、InternalExceptionを通知する")]
    public async Task SelectNameByProductUuidAsync_商品が存在しない場合_InternalExceptionを通知する()
    {
        // Arrange
        var productUuid =
            Guid.NewGuid().ToString();

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    _repository
                        .SelectNameByProductUuidAsync(
                            productUuid));

        // Assert
        Assert.AreEqual(
            $"商品UUID '{productUuid}' が見つかりません。",
            exception.Message);
    }

    /// <summary>
    /// テスト用商品と在庫をDBへ登録する
    /// </summary>
    private async Task<ProductEntity> AddProductAsync(
        int price,
        int quantity)
    {
        var token =
            Guid.NewGuid()
                .ToString("N")[..8];

        var category =
            new ProductCategoryEntity
            {
                CategoryUuid =
                    Guid.NewGuid(),

                Name =
                    $"カテゴリ_{token}"
            };

        _context.ProductCategories.Add(
            category);

        await _context.SaveChangesAsync();

        var product =
            new ProductEntity
            {
                ProductUuid =
                    Guid.NewGuid(),

                ProductCategoryId =
                    category.Id,

                Name =
                    $"商品_{token}",

                Price =
                    price,

                ImageUrl =
                    null,

                DeleteFlag =
                    0
            };

        _context.Products.Add(
            product);

        await _context.SaveChangesAsync();

        var stock =
            new ProductStockEntity
            {
                StockUuid =
                    Guid.NewGuid(),

                ProductId =
                    product.Id,

                Quantity =
                    quantity
            };

        _context.ProductStocks.Add(
            stock);

        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        return product;
    }
}