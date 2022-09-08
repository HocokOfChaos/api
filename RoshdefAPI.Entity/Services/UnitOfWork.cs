using Dapper;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using System.Collections.Concurrent;
using System.Data;

namespace RoshdefAPI.Entity.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private bool _disposed;
        private ConcurrentDictionary<string, GenericRepository> _repositories;

        public IDbConnection Connection { get => _connection; }
        public IDbTransaction Transaction { get => _transaction; }
        public UnitOfWork(IDatabaseConnectionProvider connectionProvider)
        {
            _repositories = new ConcurrentDictionary<string, GenericRepository>();
            _connection = connectionProvider.GetConnection();
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        private void ResetRepositories()
        {
            _repositories = new ConcurrentDictionary<string, GenericRepository>();
        }

        public void Register(GenericRepository repository)
        {
            _repositories.TryAdd(repository.GetType().Name, repository);
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        public async Task ExecuteNonQuery(string query)
        {
            await Connection.ExecuteAsync(
                query,
                param: null,
                transaction: Transaction
            );
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}
