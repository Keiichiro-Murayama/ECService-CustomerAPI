using ECService_CustomerAPI.Application.Authentications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Application.Tests.Authentications;

[TestClass]
public class PasswordServiceTests
{
    /// <summary>
    /// 平文パスワードをハッシュ化できること
    /// </summary>
    [TestMethod]
    public void Hash_ReturnsHashedPassword_WhenPlainPasswordIsProvided()
    {
        // Arrange
        var passwordService = new PasswordService();
        const string password = "TestPass";

        // Act
        var hashedPassword = passwordService.Hash(password);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(hashedPassword));
        Assert.AreNotEqual(password, hashedPassword);
    }

    /// <summary>
    /// 平文パスワードとハッシュ値が一致する場合に true を返すこと
    /// </summary>
    [TestMethod]
    public void Verify_ReturnsTrue_WhenPlainPasswordAndHashedPasswordMatch()
    {
        // Arrange
        var passwordService = new PasswordService();
        const string password = "TestPass";
        var hashedPassword = passwordService.Hash(password);

        // Act
        var result = passwordService.Verify(hashedPassword, password);

        // Assert
        Assert.IsTrue(result);
    }

    /// <summary>
    /// 平文パスワードとハッシュ値が不一致の場合に false を返すこと
    /// </summary>
    [TestMethod]
    public void Verify_ReturnsFalse_WhenPlainPasswordAndHashedPasswordDoNotMatch()
    {
        // Arrange
        var passwordService = new PasswordService();
        const string correctPassword = "TestPass";
        const string wrongPassword = "WrongPass";
        var hashedPassword = passwordService.Hash(correctPassword);

        // Act
        var result = passwordService.Verify(hashedPassword, wrongPassword);

        // Assert
        Assert.IsFalse(result);
    }

}
