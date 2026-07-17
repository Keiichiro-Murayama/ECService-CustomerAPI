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

    /// <summary>
    /// 商品UUIDから在庫数を取得する
    /// </summary>
    /// <param name="productUuid"></param>
    /// <returns></returns>
    /// <exception cref="InternalException"></exception>
    public async Task<int> SelectStockByProductUuidAsync(string productUuid)
    {
        var product = await _context.Products
            .Where(p => p.ProductUuid == Guid.Parse(productUuid))
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new InternalException($"商品UUID '{productUuid}' が見つかりません。");
        }

        return product.ProductStock.Quantity;
    }

    /// <summary>
    /// 商品UUIDから在庫数を更新する(悲観的ロック)
    /// </summary>
    /// <param name="productUuid"></param>
    /// <param name="quantity"></param>
    /// <returns></returns>
    /// <exception cref="InternalException"></exception>
    public async Task UpdateProductStockAsync(string productUuid, int subtractedQuantity)
    {
        var parsedUuid = Guid.Parse(productUuid);
        var product = await _context.Products
                // FOR UPDATEを使用して悲観的ロックを取得
                .FromSqlRaw("SELECT * FROM \"Products\" WHERE \"ProductUuid\" = {0} FOR UPDATE", parsedUuid)
                .Include(p => p.ProductStock) // 関連する在庫データも一緒にロード
                .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new InternalException($"商品UUID '{productUuid}' が見つかりません。");
        }

        if (product.ProductStock.Quantity < subtractedQuantity)
        {
            throw new InternalException($"商品UUID '{productUuid}' の在庫が不足しています。");
        }

        product.ProductStock.Quantity -= subtractedQuantity;

        await _context.SaveChangesAsync();
    }
}
