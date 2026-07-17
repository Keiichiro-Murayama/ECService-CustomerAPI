using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECService_CustomerAPI.Domain.Repositories;

public interface IProductRepository
{
    /// <summary>
    /// 商品UUIDから価格を取得する
    /// </summary>
    /// <param name="productUuid"></param>
    /// <returns></returns>
    Task<int> SelectPriceByProductUuidAsync(string productUuid);

    /// <summary>
    /// 商品UUIDから在庫数を取得する
    /// </summary>
    /// <param name="productUuid"></param>
    /// <returns></returns>
    Task<int> SelectStockByProductUuidAsync(string productUuid);

    /// <summary>
    /// 商品UUIDから在庫数を減算する(悲観的ロック)
    /// </summary>
    /// <param name="productUuid"></param>
    /// <param name="subtractedQuantity"></param>
    /// <returns></returns>
    Task UpdateProductStockAsync(string productUuid, int subtractedQuantity);

}
