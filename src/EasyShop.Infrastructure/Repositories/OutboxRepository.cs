using Dapper;
using EasyShop.Application.Common.Interfaces;
using EasyShop.Domain.Common;

namespace EasyShop.Infrastructure.Repositories;

public class OutboxRepository(IUnitOfWork unitOfWork) : IOutBoxRepository
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Insert(Outbox outbox, CancellationToken cancellationToken)
    {
        var sql = "INSERT INTO OutBoxs (Id,Type,Body,IsDone) VALUES (@Id,@Type, @Body, @IsDone)";
        await unitOfWork.Connection.ExecuteAsync(sql, outbox, unitOfWork.Transaction);
    }

    public async Task<List<Outbox>> Read(int batchSize, CancellationToken cancellationToken)
    {
        var sql = "SELECT * FROM Outboxs WHERE IsDone = 0 FETCH Next @batchSize ROWS ONLY";
        return await unitOfWork.Connection.QueryFirstOrDefaultAsync<List<Outbox>>(sql, new { batchSize = batchSize }) ?? [];
    }
}