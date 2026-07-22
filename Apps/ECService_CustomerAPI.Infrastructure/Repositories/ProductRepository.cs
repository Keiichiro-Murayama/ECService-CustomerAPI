using System;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService_CustomerAPI.Infrastructure.Repositories;

/// <summary>
/// 商品に関するデータアクセスを行うリポジトリ
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context">DBコンテキスト</param>
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 商品UUIDから価格を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品価格</returns>
    /// <exception cref="InternalException">
    /// 指定した商品が存在しない場合
    /// </exception>
    public async Task<int> SelectPriceByProductUuidAsync(
        string productUuid)
    {
        var product = await _context.Products
            .Where(product =>
                product.ProductUuid == Guid.Parse(productUuid))
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new InternalException(
                $"商品UUID '{productUuid}' が見つかりません。");
        }

        return product.Price;
    }

    /// <summary>
    /// 商品UUIDから在庫数を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品在庫数</returns>
    /// <exception cref="InternalException">
    /// 指定した商品が存在しない場合
    /// </exception>
    public async Task<int> SelectStockByProductUuidAsync(
        string productUuid)
    {
        var product = await _context.Products
            .Include(product => product.ProductStock)
            .Where(product =>
                product.ProductUuid == Guid.Parse(productUuid))
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new InternalException(
                $"商品UUID '{productUuid}' が見つかりません。");
        }

        return product.ProductStock.Quantity;
    }

    /// <summary>
    /// 商品在庫を購入数量分だけ減算する。
    /// 更新前に悲観的ロックを取得する。
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <param name="subtractedQuantity">購入数量</param>
    /// <returns>非同期処理</returns>
    /// <exception cref="InternalException">
    /// 指定した商品が存在しない場合
    /// </exception>
    /// <exception cref="DomainException">
    /// 商品在庫が不足している場合
    /// </exception>
    public async Task UpdateProductStockAsync(
        string productUuid,
        int subtractedQuantity)
    {
        var parsedUuid = Guid.Parse(productUuid);

        var product = await _context.Products
            // FOR UPDATEを使用して悲観的ロックを取得
            .FromSqlRaw(
                "SELECT * FROM \"product\" " +
                "WHERE \"product_uuid\" = {0} FOR UPDATE",
                parsedUuid)
            // 関連する在庫データも一緒に取得
            .Include(product => product.ProductStock)
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new InternalException(
                $"商品UUID '{productUuid}' が見つかりません。");
        }

        if (product.ProductStock.Quantity < subtractedQuantity)
        {
            throw new DomainException(
                $"申し訳ありませんが、商品「{product.Name}」の在庫が不足しています。",
                nameof(subtractedQuantity));
        }

        product.ProductStock.Quantity -= subtractedQuantity;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 商品UUIDから商品名を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品名</returns>
    /// <exception cref="InternalException">
    /// 指定した商品が存在しない場合
    /// </exception>
    public async Task<string> SelectNameByProductUuidAsync(
        string productUuid)
    {
        var productName = await _context.Products
            .Where(product =>
                product.ProductUuid == Guid.Parse(productUuid))
            .Select(product => product.Name)
            .FirstOrDefaultAsync();

        if (productName == null)
        {
            throw new InternalException(
                $"商品UUID '{productUuid}' が見つかりません。");
        }

        return productName;
    }
}