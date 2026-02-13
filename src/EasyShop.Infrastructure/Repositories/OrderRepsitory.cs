using Dapper;
using EasyShop.Application.Common.Interfaces;
using EasyShop.Domain.OrderAggregate;

namespace EasyShop.Infrastructure.Repositories;

public class OrderRepsitory(IUnitOfWork unitOfWork) : IOrderRepository
{
    public async Task Create(Order order, CancellationToken cancellationToken)
    {
        await unitOfWork.Connection.ExecuteAsync(
            @"INSERT INTO [Orders]
          (Id, CustomerId, Status, TotalAmount)
          VALUES (@OId, @CustomerId, @Status, @TotalAmount)",
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

    public async Task ChangeStatus(Guid id, OrderStatus status, CancellationToken cancellationToken)
    {
        string sql = @"UPDATE [Orders] SET  [Status]= @status WHERE Id = @Id AND RowVersion = @RowVersion";
        var rowsAffected = await unitOfWork.Connection.ExecuteAsync(sql, new {status , id}, unitOfWork.Transaction);

        if (rowsAffected == 0)
        {
            throw new Exception("concurrency error");
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