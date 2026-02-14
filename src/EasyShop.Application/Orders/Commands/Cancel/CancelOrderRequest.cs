namespace EasyShop.Application.Orders.Commands.Cancel;

public class CancelOrderRequest : IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
}