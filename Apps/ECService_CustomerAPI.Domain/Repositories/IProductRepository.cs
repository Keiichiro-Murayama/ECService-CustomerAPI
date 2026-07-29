using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
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

    //石原:追加
    /// <summary>
    /// 商品UUIDから商品名を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>商品名</returns>
    Task<string> SelectNameByProductUuidAsync(string productUuid);

    ///ogura tuika
    /// <summary>
    /// 削除されていない商品を全件取得する
    /// </summary>
    /// <returns>商品一覧</returns>
    Task<List<Product>> SelectAllAsync();

    /// <summary>
    /// 指定したカテゴリに属する商品を取得する
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <returns>指定カテゴリの商品一覧</returns>
    Task<List<Product>> SelectByCategoryAsync(string categoryUuid);


    /// <summary>
    /// 指定された商品UUIDの商品詳細を取得する
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>
    /// 商品詳細。商品が存在しない場合はnull
    /// </returns>
    Task<ProductDetail?> SelectByUuidAsync(string productUuid);
}
