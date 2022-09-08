using Dapper;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class LeaderboardPlayersRepository : LeaderboardPlayersRepositoryBase
    {
        public override string TableName => "leaderboard_players";

        public LeaderboardPlayersRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {

        }

        public override async Task<LeaderboardPlayer?> FindBySteamID(ulong steamID)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(LeaderboardPlayer.SteamID), steamID);
            return await Connection.QuerySingleOrDefaultAsync<LeaderboardPlayer>(
                QueryBuilder.BuildSelectQuery(this, $"WHERE t1.{nameof(LeaderboardPlayer.SteamID)} = @{nameof(LeaderboardPlayer.SteamID)}"),
                param: parameters,
                transaction: Transaction
            );
        }

        public override async Task ResetDailyLeaderboard()
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(LeaderboardPlayer.DayPoints), 0);
            var sql = QueryBuilder.BuildUpdateFieldQuery(this, nameof(LeaderboardPlayer.DayPoints));
            await Connection.ExecuteAsync(
                sql,
                param: parameters,
                transaction: Transaction
            );
        }

        public override async Task ResetWeeklyLeaderboard()
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(LeaderboardPlayer.WeekPoints), 0);
            var sql = QueryBuilder.BuildUpdateFieldQuery(this, nameof(LeaderboardPlayer.WeekPoints));
            await Connection.ExecuteAsync(
                sql,
                param: parameters,
                transaction: Transaction
            );
        }

        public override async Task ResetMonthlyLeaderboard()
        {
            await BulkDelete(await FindAll());
        }
    }
}
