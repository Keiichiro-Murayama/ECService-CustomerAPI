using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;

namespace ECService_CustomerAPI.Infrastructure.Adapters;

public class CustomerEntityAdapter
    : IConverter<Customer, CustomerEntity>,
      IRestorer<Customer, CustomerEntity>
{
    /// <summary>
    /// ドメインモデルをエンティティに変換する
    /// </summary>
    /// <param name="domain">顧客ドメイン</param>
    /// <returns>顧客エンティティ</returns>
    public Task<CustomerEntity> ConvertAsync(Customer domain)
    {
        return Task.FromResult(new CustomerEntity
        {
            CustomerUuid = Guid.Parse(domain.CustomerUuid),
            Name = domain.Name,

            //石原:追加 Domainの氏名カナをEntityへ設定
            NameKana = domain.NameKana,

            Address1 = domain.Address1,
            Address2 = domain.Address2,
            PhoneNumber = domain.PhoneNumber,
            MailAddress = domain.MailAddress,
            Username = domain.Username,
            Password = domain.PasswordHash,
            CreatedAt = System.DateTime.UtcNow//石原:追加 顧客アカウントの登録日時をUTCで設定
        });
    }

    /// <summary>
    /// エンティティをドメインモデルに変換する
    /// </summary>
    /// <param name="target">顧客エンティティ</param>
    /// <returns>顧客ドメイン</returns>
    public Task<Customer> RestoreAsync(CustomerEntity target)
    {
        //石原:変更 Customer.Restoreへ氏名カナを渡すように変更
        return Task.FromResult(Customer.Restore(
            target.CustomerUuid.ToString(),
            target.Name,
            target.NameKana,
            target.Address1,
            target.Address2,
            target.PhoneNumber,
            target.MailAddress,
            target.Username,
            target.Password
        ));
    }
}