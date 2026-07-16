using ECService_CustomerAPI.Domain.Exceptions;
namespace ECService_CustomerAPI.Domain.Models;

public class OrderDetail
{
    //プロパティ
    public string ProductUuid { get; private set; } = string.Empty;
    public int Count { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productUuid"></param>
    /// <param name="count"></param>
    private OrderDetail(string productUuid, int count)
    {
        this.ProductUuid = productUuid;
        this.Count = count;
    }

    /// <summary>
    /// オブジェクト生成
    /// </summary>
    /// <param name="productUuid"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static OrderDetail Create(string productUuid, int count)
    {
        ValidateUuid(productUuid);
        return new OrderDetail(productUuid, count);
    }

    public static OrderDetail Restore(string productUuid, int count)
    {
        ValidateUuid(productUuid);
        return new OrderDetail(productUuid, count);
    }

    /// <summary>
    /// UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string uuid)
    {
        if (string.IsNullOrWhiteSpace(uuid))
        {
            throw new DomainException("識別Idは必須です。", nameof(uuid));
        }

        if (!Guid.TryParse(uuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", nameof(uuid));
        }
    }
}