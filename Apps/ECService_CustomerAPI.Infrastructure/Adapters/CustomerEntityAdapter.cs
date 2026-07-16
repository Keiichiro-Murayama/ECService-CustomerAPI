using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;

namespace ECService_CustomerAPI.Infrastructure.Adapters;

public class CustomerEntityAdapter : IConverter<Customer, CustomerEntity>, IRestorer<Customer, CustomerEntity>
{
    /// <summary>
    /// ドメインモデルをエンティティに変換する
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public Task<CustomerEntity> ConvertAsync(Customer domain)
    {
        return Task.FromResult(new CustomerEntity
        {
            CustomerUuid = Guid.Parse(domain.CustomerUuid),
            Name = domain.Name,
            Address1 = domain.Address1,
            Address2 = domain.Address2,
            PhoneNumber = domain.PhoneNumber,
            MailAddress = domain.MailAddress,
            Username = domain.Username,
            Password = domain.PasswordHash
        });
    }

    /// <summary>
    /// エンティティをドメインモデルに変換する
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Task<Customer> RestoreAsync(CustomerEntity target)
    {
        return Task.FromResult(Customer.Restore(
            target.CustomerUuid.ToString(),
            target.Name,
            target.Address1,
            target.Address2,
            target.PhoneNumber,
            target.MailAddress,
            target.Username,
            target.Password
        ));
    }
}


