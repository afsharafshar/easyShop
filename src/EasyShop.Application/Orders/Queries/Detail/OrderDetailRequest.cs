namespace EasyShop.Application.Orders.Queries.Detail;

public class OrderDetailRequest : IRequest<ErrorOr<OrderDetailResponse>>
{
    public Guid Id { get; set; }
}