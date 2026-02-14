using EasyShop.Domain.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace EasyShop.Application.Orders.Commands.Cancel;

public class CancelOrderRequestHandler : IRequestHandler<CancelOrderRequest, ErrorOr<Success>>
{
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly ILogger<CancelOrderRequestHandler> _logger;

    public CancelOrderRequestHandler(IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IInventoryItemRepository inventoryItemRepository,
        ILogger<CancelOrderRequestHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(CancelOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetById(request.Id, cancellationToken);
        if (order is null)
            return OrderErrors.OrderNotFound;

        if (order.Status == OrderStatus.Cancelled)
            return Result.Success;
        
        order.Cancel();

        await _orderRepository.ChangeStatus(order.Id, order.Status, cancellationToken);

        return Result.Success;
    }
}