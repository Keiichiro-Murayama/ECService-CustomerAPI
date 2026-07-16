using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Exceptions;

namespace ECService_CustomerAPI.Domain.Models;

public class PaymentMethod
{
    //プロパティ
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    //支払い方法名の最大文字列長
    const int MaxNameLength = 100;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    private PaymentMethod(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    /// <summary>
    /// オブジェクト復元
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static PaymentMethod Restore(int id, string name)
    {
        return new PaymentMethod(id, name);
    }

    /// <summary>
    /// 支払い方法名のバリデーションチェック
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="DomainException"></exception>
    private static void ValidateName(string name)
    {
        if (name.Length > MaxNameLength)
        {
            throw new DomainException("支払い方法名が不適切です。");
        }
    }
}
