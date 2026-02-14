using Dapper;
using EasyShop.Application.Common.Interfaces;
using EasyShop.Domain.InventoryItemAggregate;
using EasyShop.Domain.ProductAggregate;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EasyShop.Infrastructure.Repositories;

public class DatabaseSeeder(IConfiguration configuration, ILogger<DatabaseSeeder> logger, IProductRepository productRepository, IInventoryItemRepository inventoryItemRepository)
{
    public async Task EnsureDatabaseMigrated(CancellationToken cancellationToken)
    {
        var script = await File.ReadAllTextAsync("Scripts/Migration.sql", cancellationToken);
        
        await using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        
        await connection.OpenAsync(cancellationToken);
        
        await using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(script, transaction: transaction);
            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("Database seeded");
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            logger.LogError(ex,"An error occurred while seeding the database");
            throw;
        }
    }
    
    public async Task EnsureDatabaseCreated(CancellationToken cancellationToken)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master" 
        };
        var masterConnectionString = builder.ToString();

        const string createDbSql = @"
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'easyshop')
            BEGIN
                CREATE DATABASE [easyshop];
            END";

        try
        {
            await using var masterConnection = new SqlConnection(masterConnectionString);
            logger.LogInformation("Connecting to master database to check/create 'easyshop' database.");
            await masterConnection.ExecuteAsync(createDbSql, cancellationToken);
            logger.LogInformation("Database creation check completed successfully.");
        }
        catch (SqlException ex)
        {
            logger.LogError(ex, "An error occurred while connecting to database");
        }
    }

    public async Task SeedDatabase(CancellationToken cancellationToken)
    {
        var id = "d7939dc4-9500-41a2-9d7b-73fa5d548d3b";
        var productId = Guid.Parse(id);
        var product = await productRepository.GetById(productId, cancellationToken);
        if (product is null)
        { 
            product = new Product(productId, "test", 100); 
            await productRepository.Add(product, cancellationToken);
        }

        var inventoryItem = await inventoryItemRepository.GetByProductId(productId, cancellationToken);
        if (inventoryItem is null)
        {
           inventoryItem = new InventoryItem(product.Id, Guid.NewGuid(), 100);
           await inventoryItemRepository.Add(inventoryItem, cancellationToken);
        }
    }

    public async Task CreateAndSeedDatabase(CancellationToken cancellationToken)
    {
        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(5);
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await EnsureDatabaseCreated(cancellationToken);
                
                await EnsureDatabaseMigrated(cancellationToken);
                
                await SeedDatabase(cancellationToken);
            
                logger.LogInformation("Database 'easyshop' is ready.");
                return;
            }
            catch (SqlException ex) when (attempt < maxRetries)
            {
                logger.LogWarning(ex, "Attempt {Attempt} failed to create database, retrying in {Delay}...", attempt, delay);
                await Task.Delay(delay, cancellationToken);
            }
        }
    }
    
}