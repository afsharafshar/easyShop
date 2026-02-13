using EasyShop.Domain.ProductAggregate;

namespace EasyShop.Application.Common.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetById(Guid id, CancellationToken cancellationToken);
    Task<bool> IsActive(Guid id, CancellationToken cancellationToken);
    
    Task Add(Product product, CancellationToken cancellationToken);
}