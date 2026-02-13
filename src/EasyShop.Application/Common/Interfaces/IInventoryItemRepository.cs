using EasyShop.Domain.InventoryItemAggregate;

namespace EasyShop.Application.Common.Interfaces;

public interface IInventoryItemRepository
{
    Task<InventoryItem?> GetByProductId(Guid productId, CancellationToken cancellationToken);
    Task Update(List<InventoryItem> inventoryItemList, CancellationToken cancellationToken);
    Task Add(InventoryItem inventoryItem, CancellationToken cancellationToken);
}