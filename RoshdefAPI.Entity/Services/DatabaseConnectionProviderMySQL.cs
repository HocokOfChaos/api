using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RoshdefAPI.Entity.Services.Core;
using System.Data;

namespace RoshdefAPI.Entity.Services
{
    public class DatabaseConnectionProviderMySQL : IDatabaseConnectionProvider
    {
        private readonly string _connectionString;

        public DatabaseConnectionProviderMySQL(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
