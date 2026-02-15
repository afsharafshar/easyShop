using Dapper;
using EasyShop.Application.Common.Interfaces;
using EasyShop.Domain.InventoryItemAggregate;

namespace EasyShop.Infrastructure.Repositories;

public class InventoryItemRepository(IUnitOfWork unitOfWork) : IInventoryItemRepository
{
    
    public async Task<InventoryItem?> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var sql = "SELECT * FROM InventoryItems WHERE ProductId = @productId";
        return await unitOfWork.Connection.QueryFirstOrDefaultAsync<InventoryItem>(sql, new { productId = productId });
    }

    public async Task Update(List<InventoryItem> inventoryItemList, CancellationToken cancellationToken)
    {
        foreach (var inventoryItem in inventoryItemList)
        {
            await Update(inventoryItem, cancellationToken);
        }
    }

    public async Task Add(InventoryItem inventoryItem, CancellationToken cancellationToken)
    {
        var sql = "INSERT INTO InventoryItems (Id,ProductId,WarehouseId,OnHandQty) VALUES (@Id,@ProductId,@WarehouseId,@OnHandQty)";
        await unitOfWork.Connection.ExecuteAsync(sql, inventoryItem, unitOfWork.Transaction);
    }

    private async Task Update(InventoryItem inventoryItem, CancellationToken cancellationToken)
    {
        string sql = @"UPDATE InventoryItems SET  ReservedQty= @ReservedQty WHERE Id = @Id AND RowVersion = @RowVersion";
        var rowsAffected = await unitOfWork.Connection.ExecuteAsync(sql, new {inventoryItem.ReservedQty, inventoryItem.Id, inventoryItem.RowVersion}, unitOfWork.Transaction);

        if (rowsAffected == 0)
        {
            throw new Exception("concurrency error");
        }
    }
    
    
}