using System.Data;
using EasyShop.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EasyShop.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IDbTransaction? _transaction;
    
    public UnitOfWork(IConfiguration configuration)
    {
        Connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _transaction = null;
    }
    
    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction => _transaction;

    public void Begin(CancellationToken cancellationToken)
    {
        if (Connection.State != ConnectionState.Open)
        {
            Connection.Open();
        }
        
        _transaction = Connection.BeginTransaction();
    }

    public void Commit(CancellationToken cancellationToken)
    {
        try
        {
            _transaction?.Commit();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Rollback(CancellationToken cancellationToken)
    {
        try
        {
            _transaction?.Rollback();
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}