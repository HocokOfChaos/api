using RoshdefAPI.Entity.Repositories.Core;
using System.Data;

namespace RoshdefAPI.Entity.Services.Core
{
    public interface IUnitOfWork : IDisposable
    {
        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; }
        public void Register(GenericRepository repository);
        public void Commit();
        public Task ExecuteNonQuery(string query);
    }
}
