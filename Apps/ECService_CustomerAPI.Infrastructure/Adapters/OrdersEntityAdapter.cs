using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Adapters;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Infrastructure.Entities;



namespace ECService_CustomerAPI.Infrastructure.Adapters;

public class OrdersEntityAdapter : IConverter<Orders, OrdersEntity>, IRestorer<Orders, OrdersEntity>
{
    /// <summary>
    /// 本アダプターでは、ConverAsyncはサポートしない
    /// </summary>
    /// <param name="domain">注文ドメインオブジェクト</param>
    /// <returns>変換後のエンティティ。CustomerId と OrdersDetails はリポジトリで設定してください。</returns>
    public Task<OrdersEntity> ConvertAsync(Orders domain)
    {
        throw new InternalException("OrdersEntityAdapter.ConvertAsync はサポートされていません。ConvertWithCustomerIdOrderDetailsAsync を使用してください。");
    }

    /// <summary>
    /// ドメインオブジェクトからエンティティへ変換（CustomerId と OrderDetailEntities 指定版）
    /// </summary>
    /// <param name="domain">注文ドメインオブジェクト</param>
    /// <param name="customerId">顧客の内部ID</param>
    /// <param name="orderDetailEntities">注文明細エンティティのリスト</param>
    /// <returns>変換後のエンティティ</returns>
    public Task<OrdersEntity> ConvertWithCustomerIdOrderDetailsAsync(Orders domain, int customerId, List<OrderDetailEntity> orderDetailEntities)
    {
        var entity = new OrdersEntity
        {
            OrderUuid = Guid.Parse(domain.OrderUuid),
            OrderDate = domain.OrderDate,
            AmountTotal = domain.AmountTotal,
            CustomerId = customerId,
            OrderStatusId = domain.OrderStatusId,
            PaymentMethodId = domain.PaymentMethodId
        };

        // 注文明細エンティティのリストを設定
        entity.OrdersDetails = orderDetailEntities;

        return Task.FromResult(entity);
    }

    /// <summary>
    /// エンティティからドメインオブジェクトへ復元
    /// </summary>
    /// <param name="entity">注文エンティティ</param>
    /// <returns>復元後のドメインオブジェクト</returns>
    public Task<Orders> RestoreAsync(OrdersEntity entity)
    {
        var orderDate = entity.OrderDate ?? throw new InternalException("OrdersEntity.OrderDate が null です。");
        // OrdersEntity.OrdersDetails を List<OrderDetail> に変換
        var orderDetails = entity.OrdersDetails.Select(od => OrderDetail.Restore(od.Product.ProductUuid.ToString(), od.Count)).ToList();
        var domain = Orders.Restore(entity.Id!.Value, entity.OrderUuid.ToString(), orderDate, entity.AmountTotal, entity.Customer.CustomerUuid.ToString(), entity.OrderStatus.Id, entity.PaymentMethodId, orderDetails);
        return Task.FromResult(domain);
    }
}
