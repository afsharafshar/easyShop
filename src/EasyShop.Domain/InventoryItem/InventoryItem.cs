using EasyShop.Domain.Common;
using ErrorOr;

namespace EasyShop.Domain.InventoryItem;

public class InventoryItem : AggregateRoot
{
    public Guid ProductId { get; private set; }
    public Guid WarehouseId { get; private set; }

    public int OnHandQty { get; private set; }
    public int ReservedQty { get; private set; }


    private InventoryItem()
    {
    }
    
    public InventoryItem(Guid productId, Guid warehouseId, int onHand)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        WarehouseId = warehouseId;
        OnHandQty = onHand;
        ReservedQty = 0;
    }
    
    public int AvailableQty => OnHandQty - ReservedQty;

    public ErrorOr<Success> Release(int qty)
    {
        if (qty <= 0)
            return InventoryItemErrors.ItemQtyMustBePositive;

        if (qty > ReservedQty)
            return InventoryItemErrors.InvalidReleaseQty;

        ReservedQty -= qty;

        return Result.Success;
    }

    public ErrorOr<Success> Reserve(int qty)
    {
        if(qty <= 0)
            return InventoryItemErrors.ItemQtyMustBePositive;
        
        if (qty > AvailableQty)
            return InventoryItemErrors.InvalidReserveQty;
        
        ReservedQty += qty;
        
        return Result.Success;
    }

}