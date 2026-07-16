using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;


namespace ECService_CustomerAPI.Domain.Repositories;

public interface IOrdersRepository
{
    /// <summary>
    /// 永続化（UUIDを返す）
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    Task<string> CreateAsync(Orders order);

    /// <summary>
    /// 顧客UUIDで顧客アカウントの注文リストを返す
    /// </summary>
    /// <param name="CustomerUuid"></param>
    /// <returns></returns>
    Task<List<Orders>> SelectByCustomerUuidAsync(string CustomerUuid);


    /// <summary>
    /// 注文UUIDで注文に含まれる注文明細のリストを返す
    /// </summary>
    /// <param name="OrderUuid"></param>
    /// <returns></returns>
    Task<List<OrderDetail>> SelectOrderDetailsByOrderUuidAsync(string OrderUuid);



}
