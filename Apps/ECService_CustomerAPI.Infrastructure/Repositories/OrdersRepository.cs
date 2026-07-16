using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Models;
using ECService_CustomerAPI.Domain.Repositories;
using ECService_CustomerAPI.Domain.Exceptions;
using ECService_CustomerAPI.Infrastructure.Entities;
using ECService_CustomerAPI.Infrastructure.Contexts;
using ECService_CustomerAPI.Infrastructure.Adapters;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ECService_CustomerAPI.Infrastructure.Repositories;

public class OrdersRepository : IOrdersRepository
{

    //プロパティ
    private readonly AppDbContext _context;
    private readonly OrdersEntityAdapter _odersAdapter;
    private readonly OrderDetailEntityAdapter _orderDetailAdapter;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context"></param>
    /// <param name="odersAdapter"></param>
    /// <param name="orderDetailAdapter"></param>
    public OrdersRepository(AppDbContext context, OrdersEntityAdapter odersAdapter, OrderDetailEntityAdapter orderDetailAdapter)
    {
        _context = context;
        _odersAdapter = odersAdapter;
        _orderDetailAdapter = orderDetailAdapter;
    }

    /// <summary>
    /// 永続化（UUIDを返す）
    /// </summary>
    /// <param name="order">保存する注文ドメインオブジェクト</param>
    /// <returns>保存された注文のUUID</returns>
    public async Task<string> CreateAsync(Orders order)
    {
        // Step 1: 顧客IDを取得
        var customerId = await _context.Customers
            .Where(c => c.CustomerUuid == Guid.Parse(order.CustomerUuid))
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (customerId <= 0)
        {
            throw new InternalException($"顧客UUID '{order.CustomerUuid}' が見つかりません。");
        }

        // Step 2: 商品IDを一括取得
        var productUuids = order.OrderDetails.Select(od => Guid.Parse(od.ProductUuid)).ToList();
        var productIdMap = await _context.Products
            .Where(p => productUuids.Contains(p.ProductUuid))
            .Select(p => new { p.ProductUuid, p.Id })
            .ToDictionaryAsync(p => p.ProductUuid, p => p.Id);

        // Step 3: 注文明細エンティティを生成
        var orderDetailEntities = new List<OrderDetailEntity>();
        foreach (var orderDetail in order.OrderDetails)
        {
            var productUuid = Guid.Parse(orderDetail.ProductUuid);
            if (!productIdMap.TryGetValue(productUuid, out var productId))
            {
                throw new InternalException($"商品UUID '{orderDetail.ProductUuid}' が見つかりません。");
            }

            var orderDetailEntity = await _orderDetailAdapter.ConvertWithProductIdAsync(orderDetail, productId);
            orderDetailEntities.Add(orderDetailEntity);
        }

        // Step 4: 注文エンティティを作成
        var entity = await _odersAdapter.ConvertWithCustomerIdOrderDetailsAsync(order, customerId, orderDetailEntities);

        // Step 5: 永続化
        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity.OrderUuid.ToString();
    }

    /// <summary>
    /// 顧客UUIDで顧客アカウントの注文リストを返す
    /// </summary>
    /// <param name="CustomerUuid">顧客UUID</param>
    /// <returns>注文ドメインオブジェクトのリスト</returns>
    public async Task<List<Orders>> SelectByCustomerUuidAsync(string CustomerUuid)
    {
        // Step 1: 注文エンティティのリストを取得（子要素も含める）
        var orderEntities = await _context.Orders
            .Where(o => o.Customer.CustomerUuid == Guid.Parse(CustomerUuid))
            .Include(o => o.OrdersDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.Customer)
            .Include(o => o.OrderStatus)
            .ToListAsync();

        // Step 2: 注文エンティティのリストをドメインオブジェクトのリストに変換
        var ordersList = new List<Orders>();
        foreach (var orderEntity in orderEntities)
        {
            var order = await _odersAdapter.RestoreAsync(orderEntity);
            ordersList.Add(order);
        }
        return ordersList;
    }

    /// <summary>
    /// 注文UUIDで注文に含まれる注文明細のリストを返す
    /// </summary>
    /// <param name="OrderUuid">注文UUID</param>
    /// <returns>注文明細ドメインオブジェクトのリスト</returns>
    public async Task<List<OrderDetail>> SelectOrderDetailsByOrderUuidAsync(string OrderUuid)
    {
        // Step 1: 注文IDを取得
        var orderId = await _context.Orders
            .Where(o => o.OrderUuid == Guid.Parse(OrderUuid))
            .Select(o => o.Id)
            .FirstOrDefaultAsync();

        if (orderId <= 0)
        {
            return new List<OrderDetail>();
        }

        // Step 2: 注文明細エンティティのリストを取得
        var orderDetailEntities = await _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .Include(od => od.Product)
            .ToListAsync();

        // Step 3: 注文明細エンティティのリストをドメインオブジェクトのリストに変換
        var orderDetailsList = new List<OrderDetail>();
        foreach (var orderDetailEntity in orderDetailEntities)
        {
            var orderDetail = await _orderDetailAdapter.RestoreAsync(orderDetailEntity);
            orderDetailsList.Add(orderDetail);
        }
        return orderDetailsList;
    }
}
