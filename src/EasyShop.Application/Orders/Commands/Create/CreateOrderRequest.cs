namespace EasyShop.Application.Orders.Commands.Create;

public class CreateOrderRequest : IRequest<ErrorOr<CreateOrderResponse>>
{
    public Guid CustomerId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];
}

public class OrderItemRequest 
{
    public Guid ProductId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderResponse
{
    public Guid Id { get; set; }
}