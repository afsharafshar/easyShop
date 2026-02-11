using EasyShop.Domain.Common;

namespace EasyShop.Domain.OrderAggregate.Events;

public record OrderCanceledEvent(Guid orderId) : IDomainEvent;