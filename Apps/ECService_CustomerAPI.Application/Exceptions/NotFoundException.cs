namespace ECService_CustomerAPI.Application.Exceptions;

/// <summary>
/// 指定されたデータが存在しない場合に発生する例外
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// 内部例外を受け取るコンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="innerException">原因となった例外</param>
    public NotFoundException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}