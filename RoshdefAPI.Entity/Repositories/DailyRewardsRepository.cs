using Dapper;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class DailyRewardsRepository : DailyRewardsRepositoryBase
    {
        public override string TableName => "daily_rewards";

        private readonly DailyRewardsItemsRepositoryBase _dailyRewardItemRepository;

        public DailyRewardsRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder, DailyRewardsItemsRepositoryBase dailyRewardItemRepository) : base(unitOfWork, queryBuilder)
        {
            _dailyRewardItemRepository = dailyRewardItemRepository;
        }

        public override async Task<DailyReward?> Find(ulong id)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(IDataObject.ID), id);
            var result = await FindByCondition($"WHERE t1.{nameof(DailyReward.ID)} = @{nameof(DailyReward.ID)}", parameters);
            return result.FirstOrDefault();
        }

        public override async Task<IEnumerable<DailyReward>> FindAll()
        {
            return await FindByCondition(string.Empty, null);
        }

        private async Task<IEnumerable<DailyReward>> FindByCondition(string condition, object? param)
        {
            var sql = QueryBuilder.BuildJoinQuery(
                new ComplexQueryTable<DailyReward>(ComplexQueryTableJoinType.NotUsedInJoin, this),
                new ComplexQueryTable<DailyRewardItem>(ComplexQueryTableJoinType.LeftJoin, _dailyRewardItemRepository, $"ON t1.{nameof(DailyReward.ID)} = t2.{nameof(DailyRewardItem.DailyRewardID)}"),
                condition
            );
            var dailyRewardDictionary = new Dictionary<ulong, DailyReward>();
            await Connection.QueryAsync<DailyReward, DailyRewardItem, DailyReward>(
                sql,
                  (dailyReward, dailyRewardItem) =>
                  {
                      DailyReward? dailyRewardEntry;

                      if (!dailyRewardDictionary.TryGetValue(dailyReward.ID, out dailyRewardEntry))
                      {
                          dailyRewardEntry = dailyReward;
                          dailyRewardDictionary.Add(dailyReward.ID, dailyRewardEntry);
                      }
                      if (dailyRewardItem != null)
                      {
                          dailyRewardEntry.Items.Add(dailyRewardItem);
                      }
                      return dailyRewardEntry;
                  },
                param: param,
                transaction: Transaction
            );
            return dailyRewardDictionary.Values;
        }

        public override async Task<DailyReward?> FindByDayNumber(uint dayNumber)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(DailyReward.DayNumber), dayNumber);
            var result = await FindByCondition($"WHERE t1.{nameof(DailyReward.DayNumber)} = @{nameof(DailyReward.DayNumber)}", parameters);
            return result.FirstOrDefault();
        }

        public override async Task<uint> FindLastDayNumber()
        {
            var dayNumber = await Connection.QuerySingleAsync<uint?>(
                sql: QueryBuilder.BuildMaxColumnValueQuery(this, nameof(DailyReward.DayNumber)),
                param: null,
                transaction: Transaction
            );
            if(!dayNumber.HasValue)
            {
                dayNumber = 0;
            }
            return dayNumber.Value;
        }
    }
}
