using EasyShop.Domain.OrderAggregate.Events;

namespace EasyShop.Application.InventoryItems.IntegratedEvents;

public class OrderCanceledEventHandler : INotificationHandler<OrderCanceledEvent>
{
    public Task Handle(OrderCanceledEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}