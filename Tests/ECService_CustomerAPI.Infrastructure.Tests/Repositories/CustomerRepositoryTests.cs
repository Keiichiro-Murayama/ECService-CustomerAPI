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
/// CustomerRepositoryの単体テスト
/// </summary>
[TestClass]
[DoNotParallelize]
public class CustomerRepositoryTests
{
    private AppDbContext _context = null!;
    private IDbContextTransaction _transaction = null!;
    private CustomerRepository _repository = null!;

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
            new CustomerRepository(
                _context,
                new CustomerEntityAdapter());
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
    /// 顧客を正常に登録できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-001 顧客を登録した場合、DBに顧客情報が保存される")]
    public async Task CreateAsync_有効な顧客を渡した場合_DBに保存される()
    {
        // Arrange
        var customer =
            CreateCustomer();

        // Act
        var result =
            await _repository.CreateAsync(
                customer);

        // Assert
        Assert.AreEqual(
            customer.CustomerUuid,
            result);

        var savedEntity =
            await _context.Customers
                .AsNoTracking()
                .SingleAsync(entity =>
                    entity.CustomerUuid ==
                    Guid.Parse(customer.CustomerUuid));

        Assert.AreEqual(
            customer.Name,
            savedEntity.Name);

        Assert.AreEqual(
            customer.NameKana,
            savedEntity.NameKana);

        Assert.AreEqual(
            customer.Address1,
            savedEntity.Address1);

        Assert.AreEqual(
            customer.Address2,
            savedEntity.Address2);

        Assert.AreEqual(
            customer.PhoneNumber,
            savedEntity.PhoneNumber);

        Assert.AreEqual(
            customer.MailAddress,
            savedEntity.MailAddress);

        Assert.AreEqual(
            customer.Username,
            savedEntity.Username);

        Assert.AreEqual(
            customer.PasswordHash,
            savedEntity.Password);

        Assert.AreNotEqual(
            default,
            savedEntity.CreatedAt);
    }

    /// <summary>
    /// 登録済みメールアドレスで顧客を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-002 登録済みメールアドレスの場合、顧客を返す")]
    public async Task FindByMailAddressAsync_顧客が存在する場合_顧客を返す()
    {
        // Arrange
        var entity =
            await AddCustomerAsync();

        // Act
        var result =
            await _repository
                .FindByMailAddressAsync(
                    entity.MailAddress);

        // Assert
        AssertCustomer(
            entity,
            result);
    }

    /// <summary>
    /// 未登録メールアドレスの場合にnullを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-003 未登録メールアドレスの場合、nullを返す")]
    public async Task FindByMailAddressAsync_顧客が存在しない場合_nullを返す()
    {
        // Arrange
        var mailAddress =
            $"{CreateToken()}@example.com";

        // Act
        var result =
            await _repository
                .FindByMailAddressAsync(
                    mailAddress);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 登録済み電話番号で顧客を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-004 登録済み電話番号の場合、顧客を返す")]
    public async Task FindByPhoneNumberAsync_顧客が存在する場合_顧客を返す()
    {
        // Arrange
        var entity =
            await AddCustomerAsync();

        // Act
        var result =
            await _repository
                .FindByPhoneNumberAsync(
                    entity.PhoneNumber);

        // Assert
        AssertCustomer(
            entity,
            result);
    }

    /// <summary>
    /// 未登録電話番号の場合にnullを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-005 未登録電話番号の場合、nullを返す")]
    public async Task FindByPhoneNumberAsync_顧客が存在しない場合_nullを返す()
    {
        // Arrange
        var phoneNumber =
            CreatePhoneNumber();

        // Act
        var result =
            await _repository
                .FindByPhoneNumberAsync(
                    phoneNumber);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 登録済みアカウント名で顧客を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-006 登録済みアカウント名の場合、顧客を返す")]
    public async Task FindByUsernameAsync_顧客が存在する場合_顧客を返す()
    {
        // Arrange
        var entity =
            await AddCustomerAsync();

        // Act
        var result =
            await _repository
                .FindByUsernameAsync(
                    entity.Username);

        // Assert
        AssertCustomer(
            entity,
            result);
    }

    /// <summary>
    /// 未登録アカウント名の場合にnullを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-007 未登録アカウント名の場合、nullを返す")]
    public async Task FindByUsernameAsync_顧客が存在しない場合_nullを返す()
    {
        // Arrange
        var username =
            $"user{CreateToken()}";

        // Act
        var result =
            await _repository
                .FindByUsernameAsync(
                    username);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// 登録済み顧客UUIDで顧客を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-008 登録済み顧客UUIDの場合、顧客を返す")]
    public async Task FindByUuidAsync_顧客が存在する場合_顧客を返す()
    {
        // Arrange
        var entity =
            await AddCustomerAsync();

        // Act
        var result =
            await _repository
                .FindByUuidAsync(
                    entity.CustomerUuid.ToString());

        // Assert
        AssertCustomer(
            entity,
            result);
    }

    /// <summary>
    /// 未登録顧客UUIDの場合にnullを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "UT-CUS-009 未登録顧客UUIDの場合、nullを返す")]
    public async Task FindByUuidAsync_顧客が存在しない場合_nullを返す()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid().ToString();

        // Act
        var result =
            await _repository
                .FindByUuidAsync(
                    customerUuid);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// テスト用顧客ドメインを生成する
    /// </summary>
    private static Customer CreateCustomer()
    {
        var token =
            CreateToken();

        return Customer.Restore(
            Guid.NewGuid().ToString(),
            "テスト太郎",
            "テストタロウ",
            "愛知県名古屋市西区",
            "テストマンション101号室",
            CreatePhoneNumber(),
            $"{token}@example.com",
            $"user{token}",
            "hashed-password");
    }

    /// <summary>
    /// テスト用顧客をDBへ登録する
    /// </summary>
    private async Task<CustomerEntity>
        AddCustomerAsync()
    {
        var customer =
            CreateCustomer();

        var entity =
            new CustomerEntity
            {
                CustomerUuid =
                    Guid.Parse(
                        customer.CustomerUuid),

                Name =
                    customer.Name,

                NameKana =
                    customer.NameKana,

                Address1 =
                    customer.Address1,

                Address2 =
                    customer.Address2,

                PhoneNumber =
                    customer.PhoneNumber,

                MailAddress =
                    customer.MailAddress,

                Username =
                    customer.Username,

                Password =
                    customer.PasswordHash,

                CreatedAt =
                    DateTime.UtcNow
            };

        _context.Customers.Add(
            entity);

        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        return entity;
    }

    /// <summary>
    /// 顧客情報が一致することを検証する
    /// </summary>
    private static void AssertCustomer(
        CustomerEntity expected,
        Customer? actual)
    {
        Assert.IsNotNull(actual);

        Assert.AreEqual(
            expected.CustomerUuid.ToString(),
            actual.CustomerUuid);

        Assert.AreEqual(
            expected.Name,
            actual.Name);

        Assert.AreEqual(
            expected.NameKana,
            actual.NameKana);

        Assert.AreEqual(
            expected.Address1,
            actual.Address1);

        Assert.AreEqual(
            expected.Address2,
            actual.Address2);

        Assert.AreEqual(
            expected.PhoneNumber,
            actual.PhoneNumber);

        Assert.AreEqual(
            expected.MailAddress,
            actual.MailAddress);

        Assert.AreEqual(
            expected.Username,
            actual.Username);

        Assert.AreEqual(
            expected.Password,
            actual.PasswordHash);
    }

    /// <summary>
    /// 一意な文字列を生成する
    /// </summary>
    private static string CreateToken()
    {
        return Guid.NewGuid()
            .ToString("N")[..10];
    }

    /// <summary>
    /// 一意な形式の電話番号を生成する
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
}
