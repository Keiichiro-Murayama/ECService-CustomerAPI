using System.ComponentModel.DataAnnotations;
namespace ECService_CustomerAPI.Presentation.ViewModels;
/// <summary>
/// ログインのリクエストを表す ViewModel(UC-02)
///
/// リクエストボディ(email, password)を受け取る。
/// ここでは必須チェックのみ行い、資格情報の正否は UseCase の認証処理で判定する。
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// メールアドレス(必須)
    /// </summary>
    [Required(ErrorMessage = "メールアドレスは必須項目です")]
    [EmailAddress(ErrorMessage = "有効なメールアドレスを入力してください")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "メールアドレスは4文字以上100文字以内で入力してください")]
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// パスワード(必須)
    /// </summary>
    [Required(ErrorMessage = "パスワードは必須項目です")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "パスワードは5文字以上20文字以内で入力してください")]
    public string Password { get; set; } = string.Empty;
}