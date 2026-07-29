using ECService_CustomerAPI.Domain.Exceptions;

namespace ECService_CustomerAPI.Domain.Models;

/// <summary>
/// 商品詳細を表すドメインオブジェクト
/// </summary>
public class ProductDetail
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; private set; }
        = string.Empty;

    /// <summary>
    /// 商品名
    /// </summary>
    public string Name { get; private set; }
        = string.Empty;

    /// <summary>
    /// 商品価格
    /// </summary>
    public int Price { get; private set; }

    /// <summary>
    /// 商品画像URL
    /// </summary>
    public string ImageUrl { get; private set; }
        = string.Empty;

    /// <summary>
    /// 商品在庫数
    /// </summary>
    public int Stock { get; private set; }

    /// <summary>
    /// カテゴリUUID
    /// </summary>
    public string CategoryUuid { get; private set; }
        = string.Empty;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private ProductDetail(
        string productUuid,
        string name,
        int price,
        string imageUrl,
        int stock,
        string categoryUuid)
    {
        ProductUuid = productUuid;
        Name = name;
        Price = price;
        ImageUrl = imageUrl;
        Stock = stock;
        CategoryUuid = categoryUuid;
    }

    /// <summary>
    /// DBから取得した商品詳細を復元する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <param name="name">商品名</param>
    /// <param name="price">商品価格</param>
    /// <param name="imageUrl">商品画像URL</param>
    /// <param name="stock">商品在庫数</param>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <returns>商品詳細</returns>
    public static ProductDetail Restore(
        string productUuid,
        string name,
        int price,
        string imageUrl,
        int stock,
        string categoryUuid)
    {
        ValidateProductUuid(productUuid);
        ValidateName(name);
        ValidatePrice(price);
        ValidateStock(stock);
        ValidateCategoryUuid(categoryUuid);

        return new ProductDetail(
            productUuid,
            name,
            price,
            imageUrl,
            stock,
            categoryUuid);
    }

    /// <summary>
    /// 商品UUIDを検証する
    /// </summary>
    private static void ValidateProductUuid(
        string productUuid)
    {
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

        if (name.Length < 2 || name.Length > 20)
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
        if (price < 0)
        {
            throw new DomainException(
                "商品価格は0円以上である必要があります。",
                nameof(price));
        }
    }

    /// <summary>
    /// 商品在庫数を検証する
    /// </summary>
    private static void ValidateStock(int stock)
    {
        if (stock < 0)
        {
            throw new DomainException(
                "商品在庫数は0以上である必要があります。",
                nameof(stock));
        }
    }

    /// <summary>
    /// カテゴリUUIDを検証する
    /// </summary>
    private static void ValidateCategoryUuid(
        string categoryUuid)
    {
        if (!Guid.TryParse(categoryUuid, out _))
        {
            throw new DomainException(
                "カテゴリUUIDの形式が不正です。",
                nameof(categoryUuid));
        }
    }
}