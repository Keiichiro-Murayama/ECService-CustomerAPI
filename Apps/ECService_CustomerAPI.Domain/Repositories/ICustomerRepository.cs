using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Domain.Repositories;

public interface ICustomerRepository
{
    /// <summary>
    /// 顧客のメールアドレスで検索する
    /// </summary>
    /// <param name="mailAddress">メールアドレス</param>
    /// <returns>該当する顧客。存在しない場合はnull</returns>
    Task<Customer?> FindByMailAddressAsync(string mailAddress);

    /// <summary>
    /// 顧客の電話番号で検索する
    /// </summary>
    /// <param name="phoneNumber">電話番号</param>
    /// <returns>該当する顧客。存在しない場合はnull</returns>
    Task<Customer?> FindByPhoneNumberAsync(string phoneNumber);

    //石原:追加 アカウント名の重複チェックに使用する検索処理
    /// <summary>
    /// 顧客のアカウント名で検索する
    /// </summary>
    /// <param name="username">アカウント名</param>
    /// <returns>該当する顧客。存在しない場合はnull</returns>
    Task<Customer?> FindByUsernameAsync(string username);

    /// <summary>
    /// 顧客を顧客UUIDで検索する
    /// </summary>
    /// <param name="customerUuid">顧客UUID</param>
    /// <returns>該当する顧客。存在しない場合はnull</returns>
    Task<Customer?> FindByUuidAsync(string customerUuid);

    /// <summary>
    /// 顧客を作成する
    /// </summary>
    /// <param name="customer">登録する顧客</param>
    /// <returns>作成した顧客のUUID</returns>
    Task<string> CreateAsync(Customer customer);
}