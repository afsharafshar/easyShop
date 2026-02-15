using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using EasyShop.Application.Common.Interfaces;
using EasyShop.Domain.Common;
using EasyShop.Domain.OrderAggregate;

namespace EasyShop.Infrastructure.Repositories;

public class OrderRepsitory(IUnitOfWork unitOfWork, IOutBoxRepository outBoxRepository) : IOrderRepository
{
    public async Task Create(Order order, CancellationToken cancellationToken)
    {
        await unitOfWork.Connection.ExecuteAsync(
            @"INSERT INTO [Orders]
          (Id, CustomerId, Status, TotalAmount)
          VALUES (@Id, @CustomerId, @Status, @TotalAmount)",
            order, unitOfWork.Transaction);

        foreach (var item in order.Items)
        {
            await unitOfWork.Connection.ExecuteAsync(
                @"INSERT INTO OrderItems
              (Id, OrderId, ProductId, Qty, UnitPrice)
              VALUES (@Id, @OrderId, @ProductId, @Qty, @UnitPrice)",
                item, unitOfWork.Transaction);
        }
    }

    public async Task ChangeStatus(Order order, CancellationToken cancellationToken)
    {
        string sql = @"UPDATE [Orders] SET  [Status]= @Status WHERE Id = @Id AND RowVersion = @RowVersion";
        var rowsAffected = await unitOfWork.Connection.ExecuteAsync(sql, new {order.Status,order.Id, order.RowVersion}, unitOfWork.Transaction);

        if (rowsAffected == 0)
        {
            throw new Exception("concurrency error");
        }

        var events = order.PopDomainEvents();
        foreach (var @event in events)
        {
            
            var outbox = new Outbox()
            {
                Body = JsonSerializer.Serialize(@event),
                CreeatedAt = DateTime.UtcNow,
                IsDone = false,
                Type = @event.GetType().ToString(),
                Id = Guid.NewGuid()
            };
            await outBoxRepository.Insert(outbox, cancellationToken);
        }
    }

    public async Task<Order?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order= await unitOfWork.Connection.QuerySingleOrDefaultAsync<Order>(
            "SELECT * FROM [Orders] WHERE Id = @Id",
            new { Id = id });

        if (order == null)
            return null;

        var items = await unitOfWork.Connection.QueryAsync<OrderItem>(
            "SELECT * FROM OrderItems WHERE OrderId = @OrderId",
            new { OrderId = id });

        order.LoadItems(items);

        return order;
    }
}