using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;

namespace ECService_CustomerAPI.Infrastructure.Adapters;

public class PaymentMethodEntityAdapter : IConverter<PaymentMethod, PaymentMethodEntity>, IRestorer<PaymentMethod, PaymentMethodEntity>
{

    /// <summary>
    /// ドメインオブジェクトからエンティティへ変換
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<PaymentMethodEntity> ConvertAsync(PaymentMethod model)
    {
        return await Task.FromResult(new PaymentMethodEntity
        {
            Id = model.Id,
            Name = model.Name
        });
    }

    /// <summary>
    /// エンティティからドメインオブジェクトへ復元
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<PaymentMethod> RestoreAsync(PaymentMethodEntity entity)
    {
        return await Task.FromResult(PaymentMethod.Restore(entity.Id, entity.Name));
    }

}
