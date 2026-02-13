using EasyShop.Domain.OrderAggregate;

namespace EasyShop.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task Create(Order order, CancellationToken cancellationToken);
    
    Task ChangeStatus(Guid id,OrderStatus status, CancellationToken cancellationToken);
    Task<Order?> GetById(Guid id, CancellationToken cancellationToken);
}