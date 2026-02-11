using EasyShop.Domain.Common;
using ErrorOr;

namespace EasyShop.Domain.OrderAggregate;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }

    public int Qty { get; private set; }
    public decimal UnitPrice { get; private set; }

    public decimal Total => Qty * UnitPrice;

    private OrderItem() { }

    public OrderItem(Guid orderId, Guid productId, int qty, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Qty = qty;
        UnitPrice = unitPrice;
    }
    
    public ErrorOr<Success> ChangeQuantity(int qty)
    {
        if (qty <= 0)
            return OrderErrors.OrderItemQtyMustBePositive;
        
        Qty = qty;

        return Result.Success;
    }
}