using EasyShop.Domain.Common;

namespace EasyShop.Application.Common.Interfaces;

public interface IOutBoxRepository
{
    Task Insert(Outbox outbox, CancellationToken cancellationToken);
    Task<List<Outbox>> Read(int batchSize, CancellationToken cancellationToken);
}