using Dapper;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using static RoshdefAPI.Data.Models.Config;

namespace RoshdefAPI.Entity.Repositories
{
    public class ConfigRepository : ConfigRepositoryBase
    {

        public override string TableName => "config";

        public ConfigRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {

        }

        public override async Task<Config?> GetValue(ConfigType type)
        {
            return await FindByType(type);
        }

        public override async Task<bool> SetValue(ConfigType type, string value)
        {
            var config = await FindByType(type);
            if (config != null)
            {
                config.Value = value;
                await Update(config);
                return true;
            }
            return false;
        }

        private async Task<Config?> FindByType(ConfigType type)
        {
            var condition = $"WHERE t1.{nameof(Config.Type)} = @{nameof(Config.Type)}";
            var parameters = new DynamicParameters();
            parameters.Add(nameof(Config.Type), type);
            var sql = QueryBuilder.BuildSelectQuery(this, condition);
            return await Connection.QuerySingleOrDefaultAsync<Config>(
              sql,
              param: parameters,
              transaction: Transaction
            );
        }
    }
}
