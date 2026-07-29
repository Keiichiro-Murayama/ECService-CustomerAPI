using ECService_CustomerAPI.Domain.Exceptions;

namespace ECService_CustomerAPI.Domain.Models;

/// <summary>
/// 商品一覧に表示する商品を表すドメインオブジェクト
/// </summary>
public class Product
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; private set; } = string.Empty;

    /// <summary>
    /// 商品名
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 商品価格
    /// </summary>
    public int Price { get; private set; }

    /// <summary>
    /// 商品画像URL
    /// </summary>
    public string ImageUrl { get; private set; } = string.Empty;

    /// <summary>
    /// 商品名の最小文字数
    /// </summary>
    private const int NameMinLength = 2;

    /// <summary>
    /// 商品名の最大文字数
    /// </summary>
    private const int NameMaxLength = 20;

    /// <summary>
    /// 商品価格の最小値
    /// </summary>
    private const int PriceMinValue = 0;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Product(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        ProductUuid = productUuid;
        Name = name;
        Price = price;
        ImageUrl = imageUrl;
    }

    /// <summary>
    /// DBから取得した商品を復元する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <param name="name">商品名</param>
    /// <param name="price">商品価格</param>
    /// <param name="imageUrl">商品画像URL</param>
    /// <returns>商品</returns>
    public static Product Restore(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        ValidateUuid(productUuid);
        ValidateName(name);
        ValidatePrice(price);

        return new Product(
            productUuid,
            name,
            price,
            imageUrl);
    }

    /// <summary>
    /// 商品UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string productUuid)
    {
        if (string.IsNullOrWhiteSpace(productUuid))
        {
            throw new DomainException(
                "商品UUIDは必須です。",
                nameof(productUuid));
        }

        if (!Guid.TryParse(productUuid, out _))
        {
            throw new DomainException(
                "商品UUIDの形式が不正です。",
                nameof(productUuid));
        }
    }

    /// <summary>
    /// 商品名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "商品名は必須です。",
                nameof(name));
        }

        if (name.Length < NameMinLength ||
            name.Length > NameMaxLength)
        {
            throw new DomainException(
                "商品名は2～20文字で入力してください。",
                nameof(name));
        }
    }

    /// <summary>
    /// 商品価格を検証する
    /// </summary>
    private static void ValidatePrice(int price)
    {
        if (price < PriceMinValue)
        {
            throw new DomainException(
                "商品価格は0円以上である必要があります。",
                nameof(price));
        }
    }
}