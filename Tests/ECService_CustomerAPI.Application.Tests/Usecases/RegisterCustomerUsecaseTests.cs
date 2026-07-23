using ECService_CustomerAPI.Application.Authentications;
using ECService_CustomerAPI.Application.Exceptions;
using ECService_CustomerAPI.Application.Usecases.Imps;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegisterCustomerInput = (
    string Name,
    string NameKana,
    string Address1,
    string Address2,
    string PhoneNumber,
    string MailAddress,
    string Username,
    string Password);

namespace ECService_CustomerAPI.Application.Tests.Usecases;

/// <summary>
/// 顧客アカウント登録Usecaseの単体テスト。
/// Domain層の個別バリデーションと実DBは対象外とする。
/// </summary>
[TestClass]
public class RegisterCustomerUsecaseTests
{
    private const string CustomerUuid =
        "11111111-1111-1111-1111-111111111111";

    private const string PasswordHash =
        "HASHED_PASSWORD";

    private Mock<ICustomerRepository>
        _customerRepository = null!;

    private Mock<IPasswordService>
        _passwordService = null!;

    private RegisterCustomerUsecase _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _customerRepository =
            new Mock<ICustomerRepository>(
                MockBehavior.Strict);

        _passwordService =
            new Mock<IPasswordService>(
                MockBehavior.Strict);

        _sut = new RegisterCustomerUsecase(
            _customerRepository.Object,
            _passwordService.Object);
    }

    /// <summary>
    /// UT-RC-027
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_重複がない場合_検索とハッシュ化と登録を順に実行してUUIDを返す()
    {
        // Arrange
        var sequence = new MockSequence();

        _customerRepository
            .InSequence(sequence)
            .Setup(repository =>
                repository.FindByMailAddressAsync(
                    "taro@example.com"))
            .ReturnsAsync((Customer?)null);

        _customerRepository
            .InSequence(sequence)
            .Setup(repository =>
                repository.FindByUsernameAsync(
                    "taro123"))
            .ReturnsAsync((Customer?)null);

        _customerRepository
            .InSequence(sequence)
            .Setup(repository =>
                repository.FindByPhoneNumberAsync(
                    "03-1111-2222"))
            .ReturnsAsync((Customer?)null);

        _passwordService
            .InSequence(sequence)
            .Setup(service =>
                service.Hash("Password123"))
            .Returns(PasswordHash);

        Customer? createdCustomer = null;

        _customerRepository
            .InSequence(sequence)
            .Setup(repository =>
                repository.CreateAsync(
                    It.IsAny<Customer>()))
            .Callback<Customer>(
                customer =>
                    createdCustomer = customer)
            .ReturnsAsync(CustomerUuid);

        var input = CreateValidInputWithSpace();

        // Act
        var actualCustomerUuid =
            await _sut.ExecuteAsync(input);

        // Assert
        Assert.AreEqual(
            CustomerUuid,
            actualCustomerUuid);
        Assert.IsNotNull(createdCustomer);
        Assert.AreEqual(
            "山田 太郎",
            createdCustomer!.Name);
        Assert.AreEqual(
            "ヤマダ タロウ",
            createdCustomer.NameKana);
        Assert.AreEqual(
            "東京都渋谷区1-11-11",
            createdCustomer.Address1);
        Assert.AreEqual(
            "マンション101号室",
            createdCustomer.Address2);
        Assert.AreEqual(
            "03-1111-2222",
            createdCustomer.PhoneNumber);
        Assert.AreEqual(
            "taro@example.com",
            createdCustomer.MailAddress);
        Assert.AreEqual(
            "taro123",
            createdCustomer.Username);
        Assert.AreEqual(
            PasswordHash,
            createdCustomer.PasswordHash);

        _customerRepository.Verify(
            repository =>
                repository.FindByMailAddressAsync(
                    input.MailAddress),
            Times.Once);
        _customerRepository.Verify(
            repository =>
                repository.FindByUsernameAsync(
                    input.Username),
            Times.Once);
        _customerRepository.Verify(
            repository =>
                repository.FindByPhoneNumberAsync(
                    input.PhoneNumber),
            Times.Once);
        _passwordService.Verify(
            service =>
                service.Hash(input.Password),
            Times.Once);
        _customerRepository.Verify(
            repository =>
                repository.CreateAsync(
                    It.IsAny<Customer>()),
            Times.Once);
    }

    /// <summary>
    /// UT-RC-028
    /// </summary>
    [TestMethod]
    [DataRow("MailAddress", "このメールアドレスは既に登録されています。")]
    [DataRow("Username", "このアカウント名は既に登録されています。")]
    [DataRow("PhoneNumber", "この電話番号は既に登録されています。")]
    public async Task ExecuteAsync_識別情報が重複する場合_ConflictExceptionを送出して後続処理を行わない(
    string duplicateTarget,
    string expectedMessage)
    {
        // Arrange
        var existingCustomer =
            CreateExistingCustomer();

        _customerRepository
            .Setup(repository =>
                repository.FindByMailAddressAsync(
                    "taro@example.com"))
            .ReturnsAsync(
                duplicateTarget == "MailAddress"
                    ? existingCustomer
                    : null);

        _customerRepository
            .Setup(repository =>
                repository.FindByUsernameAsync(
                    "taro123"))
            .ReturnsAsync(
                duplicateTarget == "Username"
                    ? existingCustomer
                    : null);

        _customerRepository
            .Setup(repository =>
                repository.FindByPhoneNumberAsync(
                    "03-1111-2222"))
            .ReturnsAsync(
                duplicateTarget == "PhoneNumber"
                    ? existingCustomer
                    : null);

        var input = CreateValidInput();

        // Act
        var exception =
            await Assert.ThrowsAsync<ConflictException>(
                () => _sut.ExecuteAsync(input));

        // Assert
        Assert.AreEqual(
          expectedMessage,
          exception.Message);

        _customerRepository.Verify(
            repository =>
                repository.FindByMailAddressAsync(
                    input.MailAddress),
            Times.Once);
        _customerRepository.Verify(
            repository =>
                repository.FindByUsernameAsync(
                    input.Username),
            Times.Once);
        _customerRepository.Verify(
            repository =>
                repository.FindByPhoneNumberAsync(
                    input.PhoneNumber),
            Times.Once);
        _passwordService.Verify(
            service =>
                service.Hash(It.IsAny<string>()),
            Times.Never);
        _customerRepository.Verify(
            repository =>
                repository.CreateAsync(
                    It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// UT-RC-029
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_正常な入力の場合_平文パスワードをハッシュ化してCustomerへ設定する()
    {
        // Arrange
        SetupNoDuplicates();

        _passwordService
            .Setup(service =>
                service.Hash("Password123"))
            .Returns(PasswordHash);

        Customer? actualCustomer = null;

        _customerRepository
            .Setup(repository =>
                repository.CreateAsync(
                    It.IsAny<Customer>()))
            .Callback<Customer>(
                customer =>
                    actualCustomer = customer)
            .ReturnsAsync(CustomerUuid);

        var input = CreateValidInput();

        // Act
        await _sut.ExecuteAsync(input);

        // Assert
        Assert.IsNotNull(actualCustomer);
        Assert.AreEqual(
            PasswordHash,
            actualCustomer!.PasswordHash);

        _passwordService.Verify(
            service =>
                service.Hash("Password123"),
            Times.Once);
    }

    /// <summary>
    /// UT-RC-030
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_顧客登録で例外が発生した場合_例外を握りつぶさず呼出元へ伝播する()
    {
        // Arrange
        SetupNoDuplicates();

        _passwordService
            .Setup(service =>
                service.Hash("Password123"))
            .Returns(PasswordHash);

        _customerRepository
            .Setup(repository =>
                repository.CreateAsync(
                    It.IsAny<Customer>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "顧客登録に失敗しました。"));

        var input = CreateValidInput();

        // Act
        var exception =
            await Assert.ThrowsAsync<
                InvalidOperationException>(
                () => _sut.ExecuteAsync(input));

        // Assert
        Assert.AreEqual(
            "顧客登録に失敗しました。",
            exception.Message);
    }

    private void SetupNoDuplicates()
    {
        _customerRepository
            .Setup(repository =>
                repository.FindByMailAddressAsync(
                    "taro@example.com"))
            .ReturnsAsync((Customer?)null);

        _customerRepository
            .Setup(repository =>
                repository.FindByUsernameAsync(
                    "taro123"))
            .ReturnsAsync((Customer?)null);

        _customerRepository
            .Setup(repository =>
                repository.FindByPhoneNumberAsync(
                    "03-1111-2222"))
            .ReturnsAsync((Customer?)null);
    }

    private static RegisterCustomerInput
        CreateValidInput()
    {
        return (
            Name: "山田太郎",
            NameKana: "ヤマダタロウ",
            Address1: "東京都渋谷区1-11-11",
            Address2: "マンション101号室",
            PhoneNumber: "03-1111-2222",
            MailAddress: "taro@example.com",
            Username: "taro123",
            Password: "Password123");
    }

    private static RegisterCustomerInput
        CreateValidInputWithSpace()
    {
        return (
            Name: "山田 太郎",
            NameKana: "ヤマダ タロウ",
            Address1: "東京都渋谷区1-11-11",
            Address2: "マンション101号室",
            PhoneNumber: "03-1111-2222",
            MailAddress: "taro@example.com",
            Username: "taro123",
            Password: "Password123");
    }

    private static Customer CreateExistingCustomer()
    {
        return Customer.Restore(
            "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
            "既存顧客",
            "キソンコキャク",
            "東京都千代田区1-1",
            string.Empty,
            "090-1234-5678",
            "existing@example.com",
            "existinguser",
            "existinghash");
    }
}