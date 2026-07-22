namespace ECService_CustomerAPI.Presentation.ViewModels;

/// <summary>
/// 顧客アカウント情報取得APIのレスポンス
/// </summary>
public class CustomerInfoResponse
{
    /// <summary>
    /// 顧客UUID
    /// </summary>
    public string CustomerUuid { get; set; } = string.Empty;

    /// <summary>
    /// 氏名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 氏名カナ
    /// </summary>
    public string NameKana { get; set; } = string.Empty;

    /// <summary>
    /// 住所1
    /// </summary>
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// 住所2
    /// </summary>
    public string Address2 { get; set; } = string.Empty;

    /// <summary>
    /// 電話番号
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string MailAddress { get; set; } = string.Empty;

    /// <summary>
    /// アカウント名
    /// </summary>
    public string Username { get; set; } = string.Empty;
}