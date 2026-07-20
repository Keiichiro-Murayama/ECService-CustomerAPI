using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Infrastructure.Entities;
using ECService_CustomerAPI.Infrastructure.Contexts;
using ECService_CustomerAPI.Infrastructure.Adapters;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ECService_CustomerAPI.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    //プロパティ
    private readonly AppDbContext _context;
    private readonly CustomerEntityAdapter _customerAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context"></param>
    /// <param name="customerAdapter"></param>
    public CustomerRepository(AppDbContext context, CustomerEntityAdapter customerAdapter)
    {
        _context = context;
        _customerAdapter = customerAdapter;
    }

    /// <summary>
    /// 顧客を作成する
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    public async Task<string> CreateAsync(Customer customer)
    {
        var customerEntity = await _customerAdapter.ConvertAsync(customer);
        _context.Customers.Add(customerEntity);
        await _context.SaveChangesAsync();
        return customerEntity.CustomerUuid.ToString();
    }


    /// <summary>
    /// 顧客のメールアドレスで検索する
    /// </summary>
    /// <param name="mailAddress"></param>
    /// <returns></returns>
    public async Task<Customer?> FindByMailAddressAsync(string mailAddress)
    {
        var customerEntity = await _context.Customers.FirstOrDefaultAsync(c => c.MailAddress == mailAddress);
        if (customerEntity == null)
        {
            return null;
        }
        var customer = await _customerAdapter.RestoreAsync(customerEntity);
        return customer;
    }

    /// <summary>
    /// 顧客の電話番号で検索する
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<Customer?> FindByPhoneNumberAsync(string phoneNumber)
    {
        var customerEntity = await _context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        if (customerEntity == null)
        {
            return null;
        }
        var customer = await _customerAdapter.RestoreAsync(customerEntity);
        return customer;
    }

    //石原:追加 アカウント名の重複チェックに使用する検索処理
    /// <summary>
    /// 顧客のアカウント名で検索する
    /// </summary>
    /// <param name="username">アカウント名</param>
    /// <returns>該当する顧客。存在しない場合はnull</returns>
    public async Task<Customer?> FindByUsernameAsync(string username)
    {
        var customerEntity =
            await _context.Customers.FirstOrDefaultAsync(
                customer => customer.Username == username);

        if (customerEntity == null)
        {
            return null;
        }

        var customer =
            await _customerAdapter.RestoreAsync(customerEntity);

        return customer;
    }

    /// <summary>
    ///   顧客を顧客UUIDで検索する
    /// </summary>
    /// <param name="customerUuid"></param>
    /// <returns></returns>
    public async Task<Customer?> FindByUuidAsync(string customerUuid)
    {
        var customerEntity = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerUuid.ToString() == customerUuid);
        if (customerEntity == null)
        {
            return null;
        }
        var customer = await _customerAdapter.RestoreAsync(customerEntity);
        return customer;
    }
}
