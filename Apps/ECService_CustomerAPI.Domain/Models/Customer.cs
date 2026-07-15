using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Exceptions;

namespace ECService_CustomerAPI.Domain.Models;

public class Customer
{
    //プロパティ
    public string? CustomerUuid { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Address1 { get; private set; } = string.Empty;
    public string Address2 { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string MailAddress { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string? Password { get; private set; }  //平文パスワード、DBには保存しない
    public string PasswordHash { get; private set; } = string.Empty; //ハッシュ化されたパスワード、DBに保存する



    ///バリデーション条件
    private const int NameMaxLength = 20;
    private const int AddressMaxLength = 100;
    private const int PhoneNumberMaxLength = 20;
    private const int MailAddressMaxLength = 200;
    private const int UserNameMaxLength = 30;
    private const int PasswordMinLength = 5;
    private const int PasswordMaxLength = 20;
    private const int PasswordHashMaxLength = 255;

    /// <summary>
    /// 新規作成用コンストラクタ
    /// </summary>
    /// <param name="name"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    private Customer(string customerUuid, string name, string address1, string address2, string phoneNumber, string mailAddress, string userName, string password, string passwordHash)
    {
        this.CustomerUuid = customerUuid;
        this.Name = name;
        this.Address1 = address1;
        this.Address2 = address2;
        this.PhoneNumber = phoneNumber;
        this.MailAddress = mailAddress;
        this.UserName = userName;
        this.Password = password;
        this.PasswordHash = passwordHash;
    }

    /// <summary>
    /// 復元用コンストラクタ
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <param name="name"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="userName"></param>
    /// <param name="passwordHash"></param>
    private Customer(string customerUuid, string name, string address1, string address2, string phoneNumber, string mailAddress, string userName, string passwordHash)
    {
        this.CustomerUuid = customerUuid;
        this.Name = name;
        this.Address1 = address1;
        this.Address2 = address2;
        this.PhoneNumber = phoneNumber;
        this.MailAddress = mailAddress;
        this.UserName = userName;
        this.PasswordHash = passwordHash;
    }



    /// <summary>
    /// 新規作成（平文パスワードを要求する）
    /// </summary>
    /// <param name="name"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="passwordHash"></param>
    /// <returns></returns>
    public static Customer Create(string name, string address1, string address2, string phoneNumber, string mailAddress, string userName, string password, string passwordHash)
    {
        var customerUuid = Guid.NewGuid().ToString();
        //バリデーション
        ValidateName(name);
        ValidateAddress1(address1);
        ValidateAddress2(address2);
        ValidatePhoneNumber(phoneNumber);
        ValidateMailAddress(mailAddress);
        ValidateUserName(userName);
        ValidatePassword(password);
        return new Customer(customerUuid, name, address1, address2, phoneNumber, mailAddress, userName, password, passwordHash);
    }

    /// <summary>
    /// 復元メソッド（平文パスワードを要求しない）
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <param name="name"></param>
    /// <param name="address1"></param>
    /// <param name="address2"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="mailAddress"></param>
    /// <param name="userName"></param>
    /// <param name="passwordHash"></param>
    public void Restore(string customerUuid, string name, string address1, string address2, string phoneNumber, string mailAddress, string userName, string passwordHash)
    {
        ValidateUuid(customerUuid);
        ValidateName(name);
        ValidateAddress1(address1);
        ValidateAddress2(address2);
        ValidatePhoneNumber(phoneNumber);
        ValidateMailAddress(mailAddress);
        ValidateUserName(userName);

        this.CustomerUuid = customerUuid;
        this.Name = name;
        this.Address1 = address1;
        this.Address2 = address2;
        this.PhoneNumber = phoneNumber;
        this.MailAddress = mailAddress;
        this.UserName = userName;
        this.PasswordHash = passwordHash;
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

    /// <summary>
    /// 顧客名を検証する
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("顧客名は必須です。", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException($"顧客名は{NameMaxLength}文字以内で入力してください。", nameof(name));
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
            throw new DomainException("住所は必須です。", nameof(address1));
        }

        if (address1.Length > AddressMaxLength)
        {
            throw new DomainException($"住所は{AddressMaxLength}文字以内で入力してください。", nameof(address1));
        }
    }

    public static void ValidateAddress2(string address2)
    {
        if (address2.Length > AddressMaxLength)
        {
            throw new DomainException($"住所は{AddressMaxLength}文字以内で入力してください。", nameof(address2));
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
            throw new DomainException("電話番号は必須です。", nameof(phoneNumber));
        }

        //ハイフンあり正規表現による形式チェック
        var regex = new System.Text.RegularExpressions.Regex(@"^\d{2,4}-\d{2,4}-\d{3,4}$");

        if (!regex.IsMatch(phoneNumber))
        {
            throw new DomainException("電話番号の形式が不正です。", nameof(phoneNumber));
        }

        if (phoneNumber.Length > PhoneNumberMaxLength)
        {
            throw new DomainException($"電話番号は{PhoneNumberMaxLength}文字以内で入力してください。", nameof(phoneNumber));
        }
    }
    /// <summary>
    ///     メールアドレスを検証する
    /// </summary>
    /// <param name="mailAddress"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateMailAddress(string mailAddress)
    {
        if (string.IsNullOrWhiteSpace(mailAddress))
        {
            throw new DomainException("メールアドレスは必須です。", nameof(mailAddress));
        }

        //メールアドレスの形式チェック
        var regex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!regex.IsMatch(mailAddress))
        {
            throw new DomainException("メールアドレスの形式が不正です。", nameof(mailAddress));
        }

        if (mailAddress.Length > MailAddressMaxLength)
        {
            throw new DomainException($"メールアドレスは{MailAddressMaxLength}文字以内で入力してください。", nameof(mailAddress));
        }
    }

    /// <summary>
    ///   アカウント名を検証する
    /// </summary>
    /// <param name="userName"></param>
    /// <exception cref="DomainException"></exception>
    public static void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new DomainException("アカウント名は必須です。", nameof(userName));
        }

        if (userName.Length > UserNameMaxLength)
        {
            throw new DomainException($"アカウント名は{UserNameMaxLength}文字以内で入力してください。", nameof(userName));
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
            throw new DomainException("パスワードは必須です。", nameof(password));
        }

        if (password.Length < PasswordMinLength || password.Length > PasswordMaxLength)
        {
            throw new DomainException($"パスワードは{PasswordMinLength}〜{PasswordMaxLength}文字で入力してください。", nameof(password));
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
            throw new DomainException($"パスワードは{PasswordHashMaxLength}文字以内で入力してください。", nameof(passwordHash));
        }
    }
}


