using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Adapters;
using ECService_CustomerAPI.Infrastructure.Entities;

namespace ECService_CustomerAPI.Infrastructure.Adapters;

public class OrderDetailEntityAdapter : IConverter<OrderDetail, OrderDetailEntity>, IRestorer<OrderDetail, OrderDetailEntity>
{
    /// <summary>
    /// ドメインオブジェクトからエンティティへ変換
    /// </summary>
    /// <param name="domain">注文明細ドメインオブジェクト</param>
    /// <returns>変換後のエンティティ。ProductId は呼び出し側で設定してください。</returns>
    public Task<OrderDetailEntity> ConvertAsync(OrderDetail domain)
    {
        // ProductId はリポジトリで別途設定されるため、ここでは設定しない
        var entity = new OrderDetailEntity
        {
            Count = domain.Count
        };
        return Task.FromResult(entity);
    }

    /// <summary>
    /// ドメインオブジェクトからエンティティへ変換（ProductId 指定版）
    /// </summary>
    /// <param name="domain">注文明細ドメインオブジェクト</param>
    /// <param name="productId">商品の内部ID</param>
    /// <returns>変換後のエンティティ</returns>
    public Task<OrderDetailEntity> ConvertWithProductIdAsync(OrderDetail domain, int productId)
    {
        var entity = new OrderDetailEntity
        {
            ProductId = productId,
            Count = domain.Count
        };
        return Task.FromResult(entity);
    }

    /// <summary>
    /// エンティティからドメインオブジェクトへ復元
    /// </summary>
    /// <param name="entity">注文明細エンティティ</param>
    /// <returns>復元後のドメインオブジェクト</returns>
    public Task<OrderDetail> RestoreAsync(OrderDetailEntity entity)
    {
        var domain = OrderDetail.Restore(entity.Product.ProductUuid.ToString(), entity.Count);
        return Task.FromResult(domain);
    }
}
