using System.Data;

namespace RoshdefAPI.Entity.Services.Core
{
    public interface IDatabaseConnectionProvider
    {
        public IDbConnection GetConnection();
    }
}
