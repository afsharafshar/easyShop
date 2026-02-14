namespace EasyShop.Application.Orders.Commands.Confirm;

public class ConfirmOrderRequest : IRequest<ErrorOr<Success>>
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
}

