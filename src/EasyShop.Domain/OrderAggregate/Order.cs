using EasyShop.Domain.Common;
using EasyShop.Domain.OrderAggregate.Events;
using ErrorOr;

namespace EasyShop.Domain.OrderAggregate;

public class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal TotalAmount => _items.Sum(x => x.Total);

    private Order() { }

    public Order(Guid customerId)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        CreatedAt = DateTime.Now;
        Status = OrderStatus.Draft;
    }
    
    public ErrorOr<Success> AddItem(Guid productId, int qty, decimal unitPrice)
    {
        if (Status != OrderStatus.Draft)
            return OrderErrors.OrderMustBeDraft;

        if (qty <= 0)
            return OrderErrors.OrderItemQtyMustBePositive;

        var item = new OrderItem(Id, productId, qty, unitPrice);
        _items.Add(item);

        return Result.Success;
    }

    public ErrorOr<Success> Confirm()
    {
        if (Status == OrderStatus.Confirmed)
            return Result.Success;

        if (Status != OrderStatus.Draft)
            return OrderErrors.OrderMustBeDraft;

        if (!_items.Any())
            return OrderErrors.OrderEmpty;

        Status = OrderStatus.Confirmed;
        
        return Result.Success;
    }
    
    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            return;
        
        Status = OrderStatus.Cancelled;
        
        _domainEvents.Add(new OrderCanceledEvent(Id));
    }

    public void LoadItems(IEnumerable<OrderItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }

}