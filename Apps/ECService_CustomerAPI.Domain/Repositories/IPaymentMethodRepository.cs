using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;

namespace ECService_CustomerAPI.Domain.Repositories;

public interface IPaymentMethodRepository
{
    /// <summary>
    /// すべての支払い方法を取得する
    /// </summary>
    /// <returns></returns>
    Task<List<PaymentMethod>> SelectAllAsync();

}
