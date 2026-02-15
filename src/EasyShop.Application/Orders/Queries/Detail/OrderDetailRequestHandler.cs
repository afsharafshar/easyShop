using EasyShop.Domain.OrderAggregate;

namespace EasyShop.Application.Orders.Queries.Detail;

public class OrderDetailRequestHandler(IOrderRepository orderRepository)
    : IRequestHandler<OrderDetailRequest, ErrorOr<OrderDetailResponse>>
{
    public async Task<ErrorOr<OrderDetailResponse>> Handle(OrderDetailRequest request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetById(request.Id, cancellationToken);
        if (order is null)
            return OrderErrors.OrderNotFound;

        var response = new OrderDetailResponse()
        {
            CreatedAt = order.CreatedAt,
            CustomerId = order.CustomerId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(x => new OrderItemDetailResponse()
            {
                Id = x.Id,
                ProductId = x.ProductId,
                Qty = x.Qty,
                UnitPrice = x.UnitPrice,
                Total = x.Total
            }).ToList()
        };

        return response;
    }
}