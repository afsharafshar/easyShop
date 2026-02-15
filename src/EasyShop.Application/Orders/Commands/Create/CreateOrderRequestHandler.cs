using EasyShop.Domain.OrderAggregate;

namespace EasyShop.Application.Orders.Commands.Create;

public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, ErrorOr<CreateOrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public CreateOrderRequestHandler(IUnitOfWork unitOfWork, IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }
    
    public async Task<ErrorOr<CreateOrderResponse>> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerId);

        if (!request.Items.Any())
            return OrderErrors.OrderHasNoItem;
        
        foreach (var item in request.Items)
        {
            var isActive = await _productRepository.IsActive(item.ProductId, cancellationToken);
            if (!isActive)
                return OrderErrors.ProductMustBeActive;

            var result = order.AddItem(item.ProductId, item.Qty, item.UnitPrice);
            if (result.IsError)
                return result.Errors;
        }
        
        await _orderRepository.Create(order, cancellationToken);

        return new CreateOrderResponse
        {
            Id = order.Id,
        };
    }
}