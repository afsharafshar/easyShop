using Dapper;
using EasyShop.Application.Common.Interfaces;
using EasyShop.Domain.ProductAggregate;

namespace EasyShop.Infrastructure.Repositories;

public class ProductRepository(IUnitOfWork unitOfWork) : IProductRepository
{
    public async Task<Product?> GetById(Guid id, CancellationToken cancellationToken)
    {
        var sql = "SELECT * FROM Products WHERE Id = @Id";
        return await unitOfWork.Connection.QueryFirstOrDefaultAsync<Product>(sql, new { Id = id });
    }

    public async Task<bool> IsActive(Guid id, CancellationToken cancellationToken)
    {
        var sql = "SELECT IsActive FROM Products WHERE Id = @Id";
        return await unitOfWork.Connection.QueryFirstOrDefaultAsync<bool>(sql, new { Id = id });
    }
    
    public async Task Add(Product product, CancellationToken cancellationToken)
    {
        var sql = "INSERT INTO Products (Id,Name,Sku,IsActive) VALUES (@Id,@Name, @Sku, @IsActive)";
        await unitOfWork.Connection.ExecuteAsync(sql, product, unitOfWork.Transaction);
    }
}