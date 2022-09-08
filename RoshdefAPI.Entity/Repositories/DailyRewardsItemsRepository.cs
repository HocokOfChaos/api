using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class DailyRewardsItemsRepository : DailyRewardsItemsRepositoryBase
    {

        public override string TableName => "daily_rewards_items";

        public DailyRewardsItemsRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder) { }
    }
}
