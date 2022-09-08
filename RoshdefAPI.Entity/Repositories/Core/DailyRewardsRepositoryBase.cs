using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class DailyRewardsRepositoryBase : GenericRepository<DailyReward>
    {
        public DailyRewardsRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task<DailyReward?> FindByDayNumber(uint dayNumber);
        public abstract Task<uint> FindLastDayNumber();
    }
}
