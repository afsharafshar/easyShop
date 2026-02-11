using ErrorOr;

namespace EasyShop.Domain.InventoryItem;

public static class InventoryItemErrors
{
    public static readonly Error ItemQtyMustBePositive =
        Error.Validation("InventoryItemQtyMustBePositive", "Inventory Item quantity must be positive");

    public static readonly Error InvalidReleaseQty =
        Error.Validation("InventoryItemReleaseQty", "release must be less than or equal to reserved qty");

    public static readonly Error InvalidReserveQty =
        Error.Validation("InventoryItemReserveQty", "reserve must be less than or equal to available qty");
}