namespace ECService_CustomerAPI.Application.Exceptions;

/// <summary>
/// 登録しようとしたデータが既存データと重複した場合に発生する例外
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    public ConflictException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 内部例外を受け取るコンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="innerException">原因となった例外</param>
    public ConflictException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}