using System.Data;
using EasyShop.Domain.Common;

namespace EasyShop.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IDbConnection Connection { get; }
    IDbTransaction? Transaction { get; }
    
    void Begin(CancellationToken cancellationToken);
    void Commit(CancellationToken cancellationToken);
    void Rollback(CancellationToken cancellationToken);
}