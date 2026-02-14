using EasyShop.Domain.InventoryItemAggregate;
using EasyShop.Domain.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace EasyShop.Application.Orders.Commands.Confirm;

public class ConfirmOrderRequestHandler : IRequestHandler<ConfirmOrderRequest, ErrorOr<Success>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly ILogger<ConfirmOrderRequestHandler> _logger;

    public ConfirmOrderRequestHandler(IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IInventoryItemRepository inventoryItemRepository,
        ILogger<ConfirmOrderRequestHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(ConfirmOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetById(request.Id, cancellationToken);
        if (order is null)
            return OrderErrors.OrderNotFound;

        if (order.Status == OrderStatus.Confirmed)
            return Result.Success;
        
        
        var inventoryItemList = new List<InventoryItem>();

        foreach (var item in order.Items)
        {
            var isActive = await _productRepository.IsActive(item.ProductId, cancellationToken);
            if (!isActive)
                return OrderErrors.ProductMustBeActive;

            var inventory = await _inventoryItemRepository.GetByProductId(item.ProductId, cancellationToken);

            if (inventory is null)
                return InventoryItemErrors.NotFound;

            var reserveResult = inventory.Reserve(item.Qty);
            if(reserveResult.IsError)
                return reserveResult.Errors;
            
            inventoryItemList.Add(inventory);
        }

        var confirmResult = order.Confirm();
        if (confirmResult.IsError)
            return confirmResult.Errors;

        try
        { 
            _unitOfWork.Begin(cancellationToken);
            
           await _inventoryItemRepository.Update(inventoryItemList, cancellationToken);
           
           await _orderRepository.ChangeStatus(order.Id, order.Status, cancellationToken);
           
           _unitOfWork.Commit(cancellationToken);
        }
        catch (Exception e)
        {
            _unitOfWork.Rollback(cancellationToken);
            _logger.LogError(e, "error in confirm order");
            throw;
        }
        
        return Result.Success;
    }
}