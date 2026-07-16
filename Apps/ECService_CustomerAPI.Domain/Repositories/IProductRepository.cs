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

}
