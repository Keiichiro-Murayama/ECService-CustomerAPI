using System.ComponentModel.DataAnnotations;
using ECService_CustomerAPI.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService_CustomerAPI.Presentation.Tests.ViewModels;

/// <summary>
/// 顧客アカウント登録リクエストの単体テスト。
/// Domain層、HTTP通信、DI、EF Core、実DBは対象外とする。
/// </summary>
[TestClass]
public class RegisterCustomerRequestTests
{
    private const string NameSpaceError =
        "氏名のスペースは文字と文字の間に1つだけ入力してください。";

    private const string NameKanaFormatError =
        "氏名カナは全角カナで入力し、スペースは文字と文字の間に1つだけ入力してください。";

    /// <summary>
    /// UT-RC-001
    /// </summary>
    [TestMethod]
    public void Validate_氏名と氏名カナにスペースがない正常値の場合_検証が成功する()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var validationResults = Validate(request);

        // Assert
        Assert.HasCount(0, validationResults);
    }

    /// <summary>
    /// UT-RC-002
    /// </summary>
    [TestMethod]
    [DataRow("山田 太郎", "ヤマダ タロウ")]
    [DataRow("山田　太郎", "ヤマダ　タロウ")]
    public void Validate_氏名と氏名カナの文字間にスペースが1つの場合_検証が成功する(
        string name,
        string nameKana)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Name = name;
        request.NameKana = nameKana;

        // Act
        var validationResults = Validate(request);

        // Assert
        Assert.HasCount(0, validationResults);
    }

    /// <summary>
    /// UT-RC-003
    /// </summary>
    [TestMethod]
    public void Validate_氏名が未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Name = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "氏名は必須項目です。");
    }

    /// <summary>
    /// UT-RC-004
    /// </summary>
    [TestMethod]
    [DataRow("山")]
    [DataRow("あああああああああああああああああああああ")]
    public void Validate_氏名が文字数範囲外の場合_文字数エラーになる(
        string name)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Name = name;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "氏名は2文字以上20文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-005
    /// </summary>
    [TestMethod]
    [DataRow(" 山田太郎")]
    [DataRow("山田太郎 ")]
    [DataRow("山田  太郎")]
    [DataRow("山田　　太郎")]
    [DataRow("山田 太郎 一郎")]
    public void Validate_氏名のスペース位置または個数が不正な場合_形式エラーになる(
        string name)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Name = name;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            NameSpaceError);
    }

    /// <summary>
    /// UT-RC-006
    /// </summary>
    [TestMethod]
    public void Validate_氏名カナが未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.NameKana = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "氏名カナは必須項目です。");
    }

    /// <summary>
    /// UT-RC-007
    /// </summary>
    [TestMethod]
    [DataRow("ア")]
    [DataRow("アアアアアアアアアアアアアアアアアアアアア")]
    public void Validate_氏名カナが文字数範囲外の場合_文字数エラーになる(
        string nameKana)
    {
        // Arrange
        var request = CreateValidRequest();
        request.NameKana = nameKana;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "氏名カナは2文字以上20文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-008
    /// </summary>
    [TestMethod]
    [DataRow("やまだタロウ")]
    [DataRow("ヤマダTARO")]
    [DataRow("ﾔﾏﾀﾞﾀﾛｳ")]
    public void Validate_氏名カナに全角カナ以外を含む場合_形式エラーになる(
        string nameKana)
    {
        // Arrange
        var request = CreateValidRequest();
        request.NameKana = nameKana;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            NameKanaFormatError);
    }

    /// <summary>
    /// UT-RC-009
    /// </summary>
    [TestMethod]
    [DataRow(" ヤマダタロウ")]
    [DataRow("ヤマダタロウ ")]
    [DataRow("ヤマダ  タロウ")]
    [DataRow("ヤマダ　　タロウ")]
    [DataRow("ヤマダ タロウ イチロウ")]
    public void Validate_氏名カナのスペース位置または個数が不正な場合_形式エラーになる(
        string nameKana)
    {
        // Arrange
        var request = CreateValidRequest();
        request.NameKana = nameKana;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            NameKanaFormatError);
    }

    /// <summary>
    /// UT-RC-010
    /// </summary>
    [TestMethod]
    public void Validate_住所1が未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Address1 = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "住所1は必須項目です。");
    }

    /// <summary>
    /// UT-RC-011
    /// </summary>
    [TestMethod]
    public void Validate_住所1が101文字の場合_文字数エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Address1 = new string('あ', 101);

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "住所1は100文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-012
    /// </summary>
    [TestMethod]
    public void Validate_住所2が101文字の場合_文字数エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Address2 = new string('あ', 101);

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "住所2は100文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-013
    /// </summary>
    [TestMethod]
    public void Validate_電話番号が未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.PhoneNumber = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "電話番号は必須項目です。");
    }

    /// <summary>
    /// UT-RC-014
    /// </summary>
    [TestMethod]
    [DataRow(
        "0123-4567-89012",
        "電話番号は14文字以内で入力してください。")]
    [DataRow(
        "0311112222",
        "電話番号の形式が正しくありません。")]
    [DataRow(
        "03-AAAA-2222",
        "電話番号の形式が正しくありません。")]
    public void Validate_電話番号の文字数または形式が不正な場合_対応するエラーになる(
        string phoneNumber,
        string expectedMessage)
    {
        // Arrange
        var request = CreateValidRequest();
        request.PhoneNumber = phoneNumber;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            expectedMessage);
    }

    /// <summary>
    /// UT-RC-015
    /// </summary>
    [TestMethod]
    public void Validate_メールアドレスが未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.MailAddress = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "メールアドレスは必須項目です。");
    }

    /// <summary>
    /// UT-RC-016
    /// </summary>
    [TestMethod]
    [DataRow("TOO_SHORT")]
    [DataRow("TOO_LONG")]
    public void Validate_メールアドレスが文字数範囲外の場合_文字数エラーになる(
        string pattern)
    {
        // Arrange
        var request = CreateValidRequest();
        request.MailAddress = pattern == "TOO_SHORT"
            ? "a@b"
            : $"{new string('a', 90)}@example.com";

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "メールアドレスは4文字以上100文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-017
    /// </summary>
    [TestMethod]
    public void Validate_メールアドレス形式が不正な場合_形式エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.MailAddress = "taro.example.com";

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "メールアドレスの形式が正しくありません。");
    }

    /// <summary>
    /// UT-RC-018
    /// </summary>
    [TestMethod]
    public void Validate_アカウント名が未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.AccountName = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "アカウント名は必須項目です。");
    }

    /// <summary>
    /// UT-RC-019
    /// </summary>
    [TestMethod]
    [DataRow("user")]
    [DataRow("abcdefghijklmnopqrstu")]
    public void Validate_アカウント名が文字数範囲外の場合_文字数エラーになる(
        string accountName)
    {
        // Arrange
        var request = CreateValidRequest();
        request.AccountName = accountName;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "アカウント名は20文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-020
    /// </summary>
    [TestMethod]
    [DataRow(
        "",
        "パスワードは必須項目です。")]
    [DataRow(
        "Ab12",
        "パスワードは5文字以上20文字以内で入力してください。")]
    [DataRow(
        "Abcdefghijklmnopqrstu",
        "パスワードは5文字以上20文字以内で入力してください。")]
    [DataRow(
        "Password_123",
        "パスワードは半角英数字のみで入力してください。")]
    public void Validate_パスワードが入力ルールに違反する場合_対応するエラーになる(
        string password,
        string expectedMessage)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = password;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            expectedMessage);
    }

    private static RegisterCustomerRequest CreateValidRequest()
    {
        return new RegisterCustomerRequest
        {
            Name = "山田太郎",
            NameKana = "ヤマダタロウ",
            Address1 = "東京都渋谷区1-11-11",
            Address2 = "マンション101号室",
            PhoneNumber = "03-1111-2222",
            MailAddress = "taro@example.com",
            AccountName = "taro123",
            Password = "Password123"
        };
    }

    private static List<ValidationResult> Validate(
        RegisterCustomerRequest request)
    {
        var validationResults =
            new List<ValidationResult>();

        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            validateAllProperties: true);

        return validationResults;
    }

    private static List<string> GetValidationMessages(
        RegisterCustomerRequest request)
    {
        return Validate(request)
            .Select(result =>
                result.ErrorMessage ?? string.Empty)
            .ToList();
    }
}