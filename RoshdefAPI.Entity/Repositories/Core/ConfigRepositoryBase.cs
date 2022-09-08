using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;
using static RoshdefAPI.Data.Models.Config;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class ConfigRepositoryBase : GenericRepository<Config>
    {
        public ConfigRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task<Config?> GetValue(ConfigType type);

        public abstract Task<bool> SetValue(ConfigType type, string value);
    }
}
