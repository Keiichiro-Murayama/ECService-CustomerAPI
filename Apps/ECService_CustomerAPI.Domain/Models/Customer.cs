using System;
using ECService_CustomerAPI.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ECService_CustomerAPI.Domain.Models;

public class Customer
{
    //プロパティ
    public string CustomerUuid { get; private set; }

    public string Name { get; private set; } = string.Empty;

    //石原:追加 氏名カナを保持するプロパティ
    public string NameKana { get; private set; } = string.Empty;

    public string Address1 { get; private set; } = string.Empty;

    public string Address2 { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string MailAddress { get; private set; } = string.Empty;

    public string Username { get; private set; } = string.Empty;

    //平文パスワード、DBには保存しない
    public string? Password { get; private set; }

    //ハッシュ化されたパスワード、DBに保存する
    public string PasswordHash { get; private set; } = string.Empty;

    ///バリデーション条件
    private const int NameMinLength = 2;
    private const int NameMaxLength = 20;

    //石原:追加 氏名カナの最小文字数
    private const int NameKanaMinLength = 2;

    //石原:追加 氏名カナの最大文字数
    private const int NameKanaMaxLength = 20;

    private const int AddressMaxLength = 100;

    private const int PhoneNumberMaxLength = 14;

    private const int MailAddressMinLength = 4;
    private const int MailAddressMaxLength = 100;

    private const int UsernameMinLength = 5;
    private const int UsernameMaxLength = 20;

    private const int PasswordMinLength = 5;

    private const int PasswordMaxLength = 20;

    private const int PasswordHashMaxLength = 255;

    /// <summary>
    /// 新規作成用コンストラクタ
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <param name="name"></param>
    /// <param name="nameKana"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="Username"></param>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    //石原:変更 新規作成時に氏名カナを受け取れるように引数を追加
    private Customer(
        string customerUuid,
        string name,
        string nameKana,
        string address1,
        string address2,
        string phoneNumber,
        string mailAddress,
        string Username,
        string password,
        string passwordHash)
    {
        this.CustomerUuid = customerUuid;
        this.Name = name;

        //石原:追加 受け取った氏名カナをプロパティへ設定
        this.NameKana = nameKana;

        this.Address1 = address1;
        this.Address2 = address2;
        this.PhoneNumber = phoneNumber;
        this.MailAddress = mailAddress;
        this.Username = Username;
        this.Password = password;
        this.PasswordHash = passwordHash;
    }

    /// <summary>
    /// 復元用コンストラクタ
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <param name="name"></param>
    /// <param name="nameKana"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="Username"></param>
    /// <param name="passwordHash"></param>
    //石原:変更 DBからの復元時に氏名カナを受け取れるように引数を追加
    private Customer(
        string customerUuid,
        string name,
        string nameKana,
        string address1,
        string address2,
        string phoneNumber,
        string mailAddress,
        string Username,
        string passwordHash)
    {
        this.CustomerUuid = customerUuid;
        this.Name = name;

        //石原:追加 復元した氏名カナをプロパティへ設定
        this.NameKana = nameKana;

        this.Address1 = address1;
        this.Address2 = address2;
        this.PhoneNumber = phoneNumber;
        this.MailAddress = mailAddress;
        this.Username = Username;
        this.PasswordHash = passwordHash;
    }

    /// <summary>
    /// 新規作成（平文パスワードを要求する）
    /// </summary>
    /// <param name="name"></param>
    /// <param name="nameKana"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="Username"></param>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    //石原:変更 顧客作成時に氏名カナを受け取れるように引数を追加
    public static Customer Create(
        string name,
        string nameKana,
        string address1,
        string address2,
        string phoneNumber,
        string mailAddress,
        string Username,
        string password,
        string passwordHash)
    {
        var customerUuid = Guid.NewGuid().ToString();

        //バリデーション
        ValidateName(name);

        //石原:追加 氏名カナの入力内容を検証
        ValidateNameKana(nameKana);

        ValidateAddress1(address1);
        ValidateAddress2(address2);
        ValidatePhoneNumber(phoneNumber);
        ValidateMailAddress(mailAddress);
        ValidateUsername(Username);
        ValidatePassword(password);

        //石原:変更 Customer生成時に氏名カナを渡すように変更
        return new Customer(
            customerUuid,
            name,
            nameKana,
            address1,
            address2,
            phoneNumber,
            mailAddress,
            Username,
            password,
            passwordHash);
    }

    /// <summary>
    /// 復元メソッド（平文パスワードを要求しない）
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <param name="name"></param>
    /// <param name="nameKana"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="Username"></param>
    /// <param name="passwordHash"></param>
    //石原:変更 顧客復元時に氏名カナを受け取れるように引数を追加
    public static Customer Restore(
        string customerUuid,
        string name,
        string nameKana,
        string address1,
        string address2,
        string phoneNumber,
        string mailAddress,
        string Username,
        string passwordHash)
    {
        ValidateUuid(customerUuid);
        ValidateName(name);

        //石原:追加 復元する氏名カナの内容を検証
        ValidateNameKana(nameKana);

        ValidateAddress1(address1);
        ValidateAddress2(address2);
        ValidatePhoneNumber(phoneNumber);
        ValidateMailAddress(mailAddress);
        ValidateUsername(Username);

        //石原:変更 Customer復元時に氏名カナを渡すように変更
        return new Customer(
            customerUuid,
            name,
            nameKana,
            address1,
            address2,
            phoneNumber,
            mailAddress,
            Username,
            passwordHash);
    }

    /// <summary>
    /// UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string uuid)
    {
        if (string.IsNullOrWhiteSpace(uuid))
        {
            throw new DomainException(
                "識別Idは必須です。",
                nameof(uuid));
        }

        if (!Guid.TryParse(uuid, out _))
        {
            throw new DomainException(
                "識別Idの形式が不正です。",
                nameof(uuid));
        }
    }

    /// <summary>
    /// 顧客名を検証する
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "氏名は必須です。",
                nameof(name));
        }

        if (name.Length < NameMinLength ||
            name.Length > NameMaxLength)
        {
            throw new DomainException(
                $"氏名は{NameMinLength}〜{NameMaxLength}文字で入力してください。",
                nameof(name));
        }

        // 先頭・末尾の空白は禁止
        if (name.StartsWith(" ") ||
            name.StartsWith("　") ||
            name.EndsWith(" ") ||
            name.EndsWith("　"))
        {
            throw new DomainException(
                "氏名のスペースは文字と文字の間に1つだけ入力してください。",
                nameof(name));
        }

        // 空白は1文字のみ許可
        if (Regex.IsMatch(name, @"[ 　]{2,}"))
        {
            throw new DomainException(
                "氏名のスペースは文字と文字の間に1つだけ入力してください。",
                nameof(name));
        }
    }

    /// <summary>
    /// 顧客名カナを検証する
    /// </summary>
    /// <param name="nameKana"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateNameKana(string nameKana)
    {
        if (string.IsNullOrWhiteSpace(nameKana))
        {
            throw new DomainException(
                "氏名カナは必須です。",
                nameof(nameKana));
        }

        if (nameKana.Length < NameKanaMinLength ||
            nameKana.Length > NameKanaMaxLength)
        {
            throw new DomainException(
                $"氏名カナは{NameKanaMinLength}〜{NameKanaMaxLength}文字で入力してください。",
                nameof(nameKana));
        }

        // 先頭・末尾スペース禁止
        if (nameKana.StartsWith(" ") ||
            nameKana.StartsWith("　") ||
            nameKana.EndsWith(" ") ||
            nameKana.EndsWith("　"))
        {
            throw new DomainException(
                "氏名カナは全角カナで入力し、スペースは文字と文字の間に1つだけ入力してください。",
                nameof(nameKana));
        }

        // スペースは1文字のみ許可
        if (Regex.IsMatch(nameKana, @"[ 　]{2,}"))
        {
            throw new DomainException(
                "氏名カナは全角カナで入力し、スペースは文字と文字の間に1つだけ入力してください。",
                nameof(nameKana));
        }

        // 全角カタカナ + 長音 + スペース許可
        var regex = new Regex(@"^[ァ-ヶー 　]+$");

        if (!regex.IsMatch(nameKana))
        {
            throw new DomainException(
                "氏名カナは全角カナで入力し、スペースは文字と文字の間に1つだけ入力してください。",
                nameof(nameKana));
        }
    }

    /// <summary>
    /// 住所1を検証する
    /// </summary>
    /// <param name="address1"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateAddress1(string address1)
    {
        if (string.IsNullOrWhiteSpace(address1))
        {
            throw new DomainException(
                "住所は必須です。",
                nameof(address1));
        }

        if (address1.Length > AddressMaxLength)
        {
            throw new DomainException(
                $"住所は{AddressMaxLength}文字以内で入力してください。",
                nameof(address1));
        }
    }

    /// <summary>
    /// 住所2を検証する
    /// </summary>
    /// <param name="address2"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateAddress2(string address2)
    {
        if (address2.Length > AddressMaxLength)
        {
            throw new DomainException(
                $"住所は{AddressMaxLength}文字以内で入力してください。",
                nameof(address2));
        }
    }

    /// <summary>
    /// 電話番号を検証する
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new DomainException(
                "電話番号は必須です。",
                nameof(phoneNumber));
        }

        //ハイフンあり正規表現による形式チェック
        var regex =
            new System.Text.RegularExpressions.Regex(
                @"^\d{2,4}-\d{2,4}-\d{3,4}$");

        if (!regex.IsMatch(phoneNumber))
        {
            throw new DomainException(
                "電話番号の形式が不正です。",
                nameof(phoneNumber));
        }

        if (phoneNumber.Length > PhoneNumberMaxLength)
        {
            throw new DomainException(
                $"電話番号は{PhoneNumberMaxLength}文字以内で入力してください。",
                nameof(phoneNumber));
        }
    }

    /// <summary>
    /// メールアドレスを検証する
    /// </summary>
    /// <param name="mailAddress"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateMailAddress(string mailAddress)
    {
        if (string.IsNullOrWhiteSpace(mailAddress))
        {
            throw new DomainException(
                "メールアドレスは必須です。",
                nameof(mailAddress));
        }

        //メールアドレスの形式チェック
        var regex =
            new System.Text.RegularExpressions.Regex(
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        if (!regex.IsMatch(mailAddress))
        {
            throw new DomainException(
                "メールアドレスの形式が不正です。",
                nameof(mailAddress));
        }

        if (mailAddress.Length < MailAddressMinLength ||
            mailAddress.Length > MailAddressMaxLength)
        {
            throw new DomainException(
                $"メールアドレスは{MailAddressMinLength}〜{MailAddressMaxLength}文字で入力してください。",
                nameof(mailAddress));
        }
    }

    /// <summary>
    /// アカウント名を検証する
    /// </summary>
    /// <param name="Username"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateUsername(string Username)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            throw new DomainException(
                "アカウント名は必須です。",
                nameof(Username));
        }

        if (Username.Length < UsernameMinLength ||
            Username.Length > UsernameMaxLength)
        {
            throw new DomainException(
                $"アカウント名は{UsernameMinLength}〜{UsernameMaxLength}文字で入力してください。",
                nameof(Username));
        }

        // 半角英数字チェック
        var regex = new Regex(@"^[a-zA-Z0-9]+$");

        if (!regex.IsMatch(Username))
        {
            throw new DomainException(
                "パスワードは半角英数字のみで入力してください。",
                nameof(Username));
        }
    }

    /// <summary>
    /// パスワードを検証する
    /// </summary>
    /// <param name="password"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException(
                "パスワードは必須です。",
                nameof(password));
        }

        if (password.Length < PasswordMinLength ||
            password.Length > PasswordMaxLength)
        {
            throw new DomainException(
                $"パスワードは{PasswordMinLength}〜{PasswordMaxLength}文字で入力してください。",
                nameof(password));
        }


        // 半角英数字チェック
        var regex = new Regex(@"^[a-zA-Z0-9]+$");

        if (!regex.IsMatch(password))
        {
            throw new DomainException(
                "パスワードは半角英数字のみで入力してください。",
                nameof(password));
        }
    }

    /// <summary>
    /// ハッシュ化されたパスワードを検証する
    /// </summary>
    /// <param name="passwordHash"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidatePasswordHash(string passwordHash)
    {
        if (passwordHash.Length > PasswordHashMaxLength)
        {
            throw new DomainException(
                $"パスワードは{PasswordHashMaxLength}文字以内で入力してください。",
                nameof(passwordHash));
        }
    }
}