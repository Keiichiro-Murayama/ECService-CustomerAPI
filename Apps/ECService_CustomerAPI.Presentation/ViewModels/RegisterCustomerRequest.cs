using System.ComponentModel.DataAnnotations;

namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 顧客アカウント登録APIのリクエスト
/// </summary>
public class RegisterCustomerRequest
{
    /// <summary>
    /// 氏名
    /// </summary>
    [Required(ErrorMessage = "氏名は必須項目です。")]
    [StringLength(
        20,
        ErrorMessage = "氏名は20文字以内で入力してください。")]
    public string Name { get; set; } = string.Empty;

    private string _nameKana = string.Empty;

    /// <summary>
    /// 氏名カナ
    /// </summary>
    [Required(ErrorMessage = "氏名カナは必須項目です。")]
    [StringLength(
        20,
        MinimumLength = 2,
        ErrorMessage = "氏名カナは2文字以上20文字以内で入力してください。")]
    [RegularExpression(
        @"^[ァ-ヶー]+$",
        ErrorMessage = "氏名カナは全角カナで入力してください。")]
    //石原:変更 入力時に氏名カナの前後の空白を除去する
    public string NameKana
    {
        get => _nameKana;
        set => _nameKana = value?.Trim() ?? string.Empty;
    }
    /// <summary>
    /// 住所1
    /// </summary>
    [Required(ErrorMessage = "住所1は必須項目です。")]
    [StringLength(
        100,
        ErrorMessage = "住所1は100文字以内で入力してください。")]
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// 住所2
    /// </summary>
    [StringLength(
        100,
        ErrorMessage = "住所2は100文字以内で入力してください。")]
    public string Address2 { get; set; } = string.Empty;

    /// <summary>
    /// 電話番号
    /// </summary>
    [Required(ErrorMessage = "電話番号は必須項目です。")]
    [StringLength(
        20,
        ErrorMessage = "電話番号は20文字以内で入力してください。")]
    [RegularExpression(
        @"^\d{2,4}-\d{2,4}-\d{3,4}$",
        ErrorMessage = "電話番号の形式が正しくありません。")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Required(ErrorMessage = "メールアドレスは必須項目です。")]
    [EmailAddress(
        ErrorMessage = "メールアドレスの形式が正しくありません。")]
    [StringLength(
        200,
        ErrorMessage = "メールアドレスは200文字以内で入力してください。")]
    public string MailAddress { get; set; } = string.Empty;

    /// <summary>
    /// アカウント名
    /// </summary>
    [Required(ErrorMessage = "アカウント名は必須項目です。")]
    [StringLength(
        30,
        ErrorMessage = "アカウント名は30文字以内で入力してください。")]
    public string AccountName { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required(ErrorMessage = "パスワードは必須項目です。")]
    [StringLength(
        20,
        MinimumLength = 5,
        ErrorMessage = "パスワードは5文字以上20文字以内で入力してください。")]
    public string Password { get; set; } = string.Empty;
}