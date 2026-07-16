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

public class PaymentMethodRepository : IPaymentMethodRepository
{
    //プロパティ
    private readonly AppDbContext _context;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context"></param>
    public PaymentMethodRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// すべての支払い方法を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<List<PaymentMethod>> SelectAllAsync()
    {
        var paymentMethods = await _context.PaymentMethods.ToListAsync();
        return paymentMethods.Select(pm => PaymentMethod.Restore(pm.Id, pm.Name)).ToList();
    }

}
