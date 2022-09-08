using Dapper;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class PlayersLogsRepository : PlayersLogsRepositoryBase
    {
        public override string TableName => "players_logs";

        private readonly PlayersRepositoryBase _playersRepository;

        public PlayersLogsRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder, PlayersRepositoryBase playersRepository) : base(unitOfWork, queryBuilder)
        {
            _playersRepository = playersRepository;
        }

        public override async Task Log(ulong steamID, IPlayerLogEntry logEntry)
        {
            var player = await _playersRepository.FindBySteamID(steamID, false);
            var log = new PlayerLog(logEntry, player.ID);
            await Connection.QuerySingleAsync(
                QueryBuilder.BuildInsertQuery(this),
                param: log,
                transaction: Transaction
            );
        }

        public override async Task<IEnumerable<PlayerLog>> FindLogsBySteamID(ulong steamID)
        {
            var player = await _playersRepository.FindBySteamID(steamID, false);
            var parameters = new DynamicParameters();
            parameters.Add(nameof(PlayerLog.PlayerID), player.ID);
            return await Connection.QueryAsync<PlayerLog>(
                QueryBuilder.BuildSelectQuery(this, $"WHERE t1.{nameof(PlayerLog.PlayerID)} = @{nameof(PlayerLog.PlayerID)}"),
                param: parameters,
                transaction: Transaction
            );
        }
    }
}
