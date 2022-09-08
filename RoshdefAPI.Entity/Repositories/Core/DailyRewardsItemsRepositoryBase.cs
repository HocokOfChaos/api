using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class DailyRewardsItemsRepositoryBase : GenericRepository<DailyRewardItem>
    {
        public DailyRewardsItemsRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }
    }
}
