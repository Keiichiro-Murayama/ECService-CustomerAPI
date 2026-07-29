using ECService_CustomerAPI.Domain.Exceptions;

namespace ECService_CustomerAPI.Domain.Models;

/// <summary>
/// 商品カテゴリを表すドメインオブジェクト
/// </summary>
public class ProductCategory
{
    /// <summary>
    /// カテゴリUUID
    /// </summary>
    public string CategoryUuid { get; private set; } = string.Empty;

    /// <summary>
    /// カテゴリ名
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// カテゴリ名の最大文字数
    /// </summary>
    private const int NameMaxLength = 30;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <param name="name">カテゴリ名</param>
    private ProductCategory(
        string categoryUuid,
        string name)
    {
        CategoryUuid = categoryUuid;
        Name = name;
    }

    /// <summary>
    /// DBから取得した商品カテゴリを復元する
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <param name="name">カテゴリ名</param>
    /// <returns>商品カテゴリ</returns>
    public static ProductCategory Restore(
        string categoryUuid,
        string name)
    {
        ValidateUuid(categoryUuid);
        ValidateName(name);

        return new ProductCategory(
            categoryUuid,
            name);
    }

    /// <summary>
    /// カテゴリUUIDを検証する
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    private static void ValidateUuid(string categoryUuid)
    {
        if (string.IsNullOrWhiteSpace(categoryUuid))
        {
            throw new DomainException(
                "カテゴリUUIDは必須です。",
                nameof(categoryUuid));
        }

        if (!Guid.TryParse(categoryUuid, out _))
        {
            throw new DomainException(
                "カテゴリUUIDの形式が不正です。",
                nameof(categoryUuid));
        }
    }

    /// <summary>
    /// カテゴリ名を検証する
    /// </summary>
    /// <param name="name">カテゴリ名</param>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "カテゴリ名は必須です。",
                nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException(
                "カテゴリ名は30文字以内で入力してください。",
                nameof(name));
        }
    }
}