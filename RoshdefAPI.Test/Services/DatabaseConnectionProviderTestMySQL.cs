using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RoshdefAPI.Entity.Services.Core;
using System.Data;

namespace RoshdefAPI.Test.Services
{
    public class DatabaseConnectionProviderTestMySQL : IDatabaseConnectionProvider
    {
        private string _connectionString = "server=localhost; port=3306; database=roshan_test; user=root; password=; Persist Security Info=False; Connect Timeout=300; convert zero datetime=True";

        public DatabaseConnectionProviderTestMySQL(IConfiguration config)
        {

        }

        public IDbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
