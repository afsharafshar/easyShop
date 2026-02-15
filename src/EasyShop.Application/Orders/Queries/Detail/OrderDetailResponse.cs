using EasyShop.Domain.OrderAggregate;

namespace EasyShop.Application.Orders.Queries.Detail;

public class OrderDetailResponse
{
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemDetailResponse> Items { get; set; } = [];
}

public class OrderItemDetailResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}