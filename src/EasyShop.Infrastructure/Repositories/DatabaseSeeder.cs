using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EasyShop.Infrastructure.Repositories;

public class DatabaseSeeder(IConfiguration configuration, ILogger<DatabaseSeeder> logger)
{
    public async Task EnsureDatabaseExistsAsync(CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        await connection.OpenAsync(cancellationToken);

        var script = await File.ReadAllTextAsync("Scripts/Migration.sql", cancellationToken);

        using var transaction = connection.BeginTransaction();

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
}