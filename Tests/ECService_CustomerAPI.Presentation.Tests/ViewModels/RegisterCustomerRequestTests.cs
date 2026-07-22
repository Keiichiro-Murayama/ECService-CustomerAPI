using System.ComponentModel.DataAnnotations;
using ECService_CustomerAPI.Presentation.ViewModels;

namespace ECService_CustomerAPI.Presentation.Tests.ViewModels;

/// <summary>
/// 顧客アカウント登録リクエストの単体テスト。
/// HTTP通信やDomain層の検証は対象外とする。
/// </summary>
[TestClass]
public class RegisterCustomerRequestTests
{
    /// <summary>
    /// UT-RC-001
    /// </summary>
    [TestMethod]
    public void Validate_全項目が入力ルールを満たす場合_検証が成功する()
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
    /// UT-RC-003
    /// </summary>
    [TestMethod]
    public void Validate_氏名が21文字の場合_文字数エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Name = new string('あ', 21);

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "氏名は20文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-004
    /// </summary>
    [TestMethod]
    public void NameKana_前後に空白がある場合_空白を除去して保持する()
    {
        // Arrange・Act
        var request = CreateValidRequest();
        request.NameKana = "  ヤマダタロウ  ";

        // Assert
        Assert.AreEqual(
            "ヤマダタロウ",
            request.NameKana);
    }

    /// <summary>
    /// UT-RC-005
    /// </summary>
    [TestMethod]
    [DataRow("ア")]
    [DataRow(
        "アアアアアアアアアアアアアアアアアアアアア")]
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
    /// UT-RC-006
    /// </summary>
    [TestMethod]
    public void Validate_氏名カナに全角カナ以外を含む場合_形式エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.NameKana = "やまだタロウ";

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "氏名カナは全角カナで入力してください。");
    }

    /// <summary>
    /// UT-RC-007
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
    /// UT-RC-008
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
    /// UT-RC-009
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
    /// UT-RC-010
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
    /// UT-RC-011
    /// </summary>
    [TestMethod]
    public void Validate_電話番号が21文字の場合_文字数エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.PhoneNumber = new string('1', 21);

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "電話番号は20文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-012
    /// </summary>
    [TestMethod]
    public void Validate_電話番号がハイフン付き形式でない場合_形式エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.PhoneNumber = "0311112222";

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "電話番号の形式が正しくありません。");
    }

    /// <summary>
    /// UT-RC-013
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
    /// UT-RC-014
    /// </summary>
    [TestMethod]
    public void Validate_メールアドレスが201文字を超える場合_文字数エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.MailAddress =
            $"{new string('a', 190)}@example.com";

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "メールアドレスは200文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-015
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
    /// UT-RC-016
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
    /// UT-RC-017
    /// </summary>
    [TestMethod]
    public void Validate_アカウント名が31文字の場合_文字数エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.AccountName = new string('a', 31);

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "アカウント名は30文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-018
    /// </summary>
    [TestMethod]
    public void Validate_パスワードが未入力の場合_必須エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = string.Empty;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "パスワードは必須項目です。");
    }

    /// <summary>
    /// UT-RC-019
    /// </summary>
    [TestMethod]
    [DataRow("Ab12")]
    [DataRow("Abcdefghijklmnopqrstu")]
    public void Validate_パスワードが文字数範囲外の場合_文字数エラーになる(
        string password)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = password;

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "パスワードは5文字以上20文字以内で入力してください。");
    }

    /// <summary>
    /// UT-RC-020
    /// </summary>
    [TestMethod]
    public void Validate_パスワードに半角英数字以外を含む場合_形式エラーになる()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = "Password_123";

        // Act
        var messages = GetValidationMessages(request);

        // Assert
        CollectionAssert.Contains(
            messages,
            "パスワードは半角英数字のみで入力してください。");
    }

    private static RegisterCustomerRequest CreateValidRequest()
    {
        return new RegisterCustomerRequest
        {
            Name = "山田太郎",
            NameKana = "ヤマダタロウ",
            Address1 = "東京都渋谷区1-11-11",
            Address2 = "マンション渋谷101号室",
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
            .Select(result => result.ErrorMessage ?? string.Empty)
            .ToList();
    }
}
