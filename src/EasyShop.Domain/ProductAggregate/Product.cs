using EasyShop.Domain.Common;

namespace EasyShop.Domain.ProductAggregate;

public class Product : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public int Sku { get; private set; }
    public bool IsActive { get; private set; }

    private Product()
    {
    }

    public Product(Guid id, string name, int sku)
    {
        Id = id;
        Name = name;
        Sku = sku;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    
    public void Activate() => IsActive = true;
}