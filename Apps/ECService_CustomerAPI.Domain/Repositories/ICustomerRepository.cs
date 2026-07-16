using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
namespace ECService_CustomerAPI.Domain.Repositories;

public interface ICustomerRepository
{
    /// <summary>
    ///　顧客のメールアドレスで検索する
    /// </summary>
    /// <param name="mailAddress"></param>
    /// <returns></returns>
    Task<Customer?> FindByMailAddressAsync(string mailAddress);

    /// <summary>
    /// 顧客の電話番号で検索する
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    Task<Customer?> FindByPhoneNumberAsync(string phoneNumber);

    /// <summary>
    /// 顧客を顧客UUIDで検索する
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <returns></returns>
    Task<Customer?> FindByUuidAsync(string customerUuid);

    /// <summary>
    /// 顧客を作成する
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    Task<string> CreateAsync(Customer customer);




}
