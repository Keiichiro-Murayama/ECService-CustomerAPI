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
    /// 注文UUIDと顧客UUIDで、本人の注文に含まれる注文明細のリストを返す
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <param name="customerUuid">ログイン中の顧客UUID</param>
    /// <returns>注文明細ドメインオブジェクトのリスト</returns>
    //石原:変更 他顧客の注文明細取得を防ぐため、顧客UUIDを検索条件に追加
    Task<List<OrderDetail>> SelectOrderDetailsByOrderUuidAsync(
        string orderUuid,
        string customerUuid);



}
