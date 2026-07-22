using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECService_CustomerAPI.Domain.Exceptions;


namespace ECService_CustomerAPI.Domain.Models;

public class Orders
{
    //プロパティ
    public int? Id { get; private set; }
    public string OrderUuid { get; private set; } = string.Empty;
    public DateTimeOffset? OrderDate { get; private set; }
    public int AmountTotal { get; private set; }
    public string CustomerUuid { get; private set; } = string.Empty;
    public int OrderStatusId { get; private set; }
    public int PaymentMethodId { get; private set; }
    public List<OrderDetail> OrderDetails { get; private set; } = new List<OrderDetail>();

    //StatusIDの初期値（1 : 注文済）
    private const int InitialOrderStatusId = 1;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="orderUuid"></param>
    /// <param name="amountTotal"></param>
    /// <param name="CustomerUuid"></param>
    /// <param name="orderStatusId"></param>
    /// <param name="paymentMethodId"></param>
    /// <param name="orderDetails"></param>
    private Orders(string orderUuid, int amountTotal, string CustomerUuid, int orderStatusId, int paymentMethodId, List<OrderDetail> orderDetails)
    {
        this.OrderUuid = orderUuid;
        this.AmountTotal = amountTotal;
        this.CustomerUuid = CustomerUuid;
        this.OrderStatusId = orderStatusId;
        this.PaymentMethodId = paymentMethodId;
        if (orderDetails != null)
        {
            this.OrderDetails = orderDetails;
        }
    }


    /// <summary>
    /// 新規作成
    /// </summary>
    /// <param name="amountTotal"></param>
    /// <param name="CustomerUuid"></param>
    /// <param name="orderStatusId"></param>
    /// <param name="paymentMethodId"></param>
    /// <param name="orderDetails"></param>
    /// <returns></returns>
    public static Orders Create(int amountTotal, string CustomerUuid, int paymentMethodId, List<OrderDetail> orderDetails)
    {
        var orderUuid = Guid.NewGuid().ToString();
        var orders = new Orders(orderUuid, amountTotal, CustomerUuid, InitialOrderStatusId, paymentMethodId, orderDetails);
        return orders;
    }


    //復元メソッド
    public static Orders Restore(int id, string orderUuid, DateTimeOffset OrderDate, int amountTotal, string CustomerUuid, int orderStatusId, int paymentMethodId, List<OrderDetail> orderDetails)
    {
        ValidateUuid(orderUuid);
        var orders = new Orders(orderUuid, amountTotal, CustomerUuid, orderStatusId, paymentMethodId, orderDetails);
        orders.Id = id;
        orders.OrderDate = OrderDate;
        return orders;
    }

    /// <summary>
    /// UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string uuid)
    {
        if (string.IsNullOrWhiteSpace(uuid))
        {
            throw new DomainException("識別Idは必須です。", nameof(uuid));
        }

        if (!Guid.TryParse(uuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", nameof(uuid));
        }
    }
}
