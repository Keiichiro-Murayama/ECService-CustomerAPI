using ECService_CustomerAPI.Infrastructure.Contexts;
using ECService_CustomerAPI.Infrastructure.Entities;
using ECService_CustomerAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Infrastructure.Tests.Repositories;

/// <summary>
/// PaymentMethodRepositoryの単体テスト
/// </summary>
[TestClass]
[DoNotParallelize]
public class PaymentMethodRepositoryTests
{
    private AppDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;
    private PaymentMethodRepository _repository = null!;

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
            new PaymentMethodRepository(
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
    /// 登録されているすべての支払い方法を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-PAY-001 支払い方法が登録されている場合、すべて取得する")]
    public async Task SelectAllAsync_支払い方法が登録されている場合_すべて取得する()
    {
        // Arrange
        var token =
            Guid.NewGuid()
                .ToString("N")[..8];

        var firstEntity =
            new PaymentMethodEntity
            {
                Name =
                    $"テスト支払方法A_{token}"
            };

        var secondEntity =
            new PaymentMethodEntity
            {
                Name =
                    $"テスト支払方法B_{token}"
            };

        _context.PaymentMethods.AddRange(
            firstEntity,
            secondEntity);

        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Act
        var result =
            await _repository.SelectAllAsync();

        // Assert
        var addedPaymentMethods =
            result
                .Where(paymentMethod =>
                    paymentMethod.Id == firstEntity.Id ||
                    paymentMethod.Id == secondEntity.Id)
                .OrderBy(paymentMethod =>
                    paymentMethod.Id)
                .ToList();

        Assert.HasCount(
            2,
            addedPaymentMethods);

        var firstResult =
            addedPaymentMethods.Single(
                paymentMethod =>
                    paymentMethod.Id ==
                    firstEntity.Id);

        Assert.AreEqual(
            firstEntity.Name,
            firstResult.Name);

        var secondResult =
            addedPaymentMethods.Single(
                paymentMethod =>
                    paymentMethod.Id ==
                    secondEntity.Id);

        Assert.AreEqual(
            secondEntity.Name,
            secondResult.Name);
    }
}