using ErrorOr;

namespace EasyShop.Domain.OrderAggregate;

public static class OrderErrors
{
    public static readonly Error OrderMustBeDraft =
        Error.Validation("Order.MustBeDraft", "Order must be draft to move forward.");

    public static readonly Error OrderEmpty = Error.Validation("Order.Empty", "Order must have item(s) to proceed");

    public static readonly Error OrderItemQtyMustBePositive =
        Error.Validation("Order.ItemQtyMustBePositive", "Order item quantity must be greater then zero");
    
    public static readonly Error ProductMustBeActive = Error.Validation("Order.MustHasActiveProduct", "Product must be active");
    
    public static readonly Error OrderNotFound = Error.Validation("Order.NotFound", "Order not found");
}