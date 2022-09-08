using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class LeaderboardPlayersRepositoryBase : GenericRepository<LeaderboardPlayer>
    {
        public LeaderboardPlayersRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task ResetDailyLeaderboard();
        public abstract Task ResetWeeklyLeaderboard();
        public abstract Task ResetMonthlyLeaderboard();
        public abstract Task<LeaderboardPlayer?> FindBySteamID(ulong steamID);
    }
}
