using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService_CustomerAPI.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 商品UUIDから価格を取得する
    /// </summary>
    /// <param name="productUuid"></param>
    /// <returns></returns>
    public async Task<int> SelectPriceByProductUuidAsync(string productUuid)
    {
        var product = await _context.Products
            .Where(p => p.ProductUuid == Guid.Parse(productUuid))
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new InternalException($"商品UUID '{productUuid}' が見つかりません。");
        }

        return product.Price;
    }

}
