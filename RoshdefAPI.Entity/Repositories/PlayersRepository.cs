using Dapper;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class PlayersRepository : PlayersRepositoryBase
    {
        public override string TableName => "players";

        private readonly PlayersItemsRepositoryBase _playerItemRepository;
        public PlayersRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder, PlayersItemsRepositoryBase playerItemRepository) : base(unitOfWork, queryBuilder)
        {
            _playerItemRepository = playerItemRepository;
        }

        public override async Task<Player?> Find(ulong id)
        {
            return await Find(id, true);
        }
        public override async Task<Player?> Find(ulong id, bool includeItems)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(IDataObject.ID), id);
            var result = await FindByCondition($"WHERE t1.{nameof(Player.ID)} = @{nameof(Player.ID)}", parameters, includeItems);
            return result.FirstOrDefault();
        }

        public override async Task<Player?> FindBySteamID(ulong steamID, bool includeItems)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(Player.SteamID), steamID);
            var result = await FindByCondition($"WHERE t1.{nameof(Player.SteamID)} = @{nameof(Player.SteamID)}", parameters, includeItems);
            return result.FirstOrDefault();
        }

        public override async Task<IEnumerable<Player>> FindAll()
        {
            return await FindAll(true);
        }

        public override async Task<IEnumerable<Player>> FindAll(bool includeItems)
        {
            return await FindByCondition(string.Empty, null, includeItems);
        }

        public override async Task Insert(Player entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.ID = await Connection.ExecuteScalarAsync<uint>(
                QueryBuilder.BuildInsertQuery(this),
                param: entity,
                transaction: Transaction
            );
        }

        private async Task<IEnumerable<Player>> FindByCondition(string condition, object? param, bool includeItems)
        {
            var sql = string.Empty;
            if (includeItems)
            {
                sql = QueryBuilder.BuildJoinQuery(
                    new ComplexQueryTable<Player>(ComplexQueryTableJoinType.NotUsedInJoin, this),
                    new ComplexQueryTable<PlayerItem>(ComplexQueryTableJoinType.LeftJoin, _playerItemRepository, $"ON t1.{nameof(Player.ID)} = t2.{nameof(PlayerItem.PlayerID)}"),
                    condition
                );
                var playerDictionary = new Dictionary<ulong, Player>();
                var todayDate = DateTime.Now;
                await Connection.QueryAsync<Player, PlayerItem, Player>(
                    sql,
                    (player, playerItem) =>
                    {
                        Player? playerEntry;

                        if (!playerDictionary.TryGetValue(player.ID, out playerEntry))
                        {
                            playerEntry = player;
                            playerDictionary.Add(player.ID, playerEntry);
                        }
                        if (playerItem != null)
                        {
                            TimeSpan? itemDuration = playerItem.ExpireDate.HasValue ? (playerItem.ExpireDate.Value - todayDate) : null;
                            playerEntry.AddItem(playerItem, itemDuration);
                        }
                        return player;
                    },
                    param: param,
                    transaction: Transaction
                );
                return playerDictionary.Values;
            }
            sql = QueryBuilder.BuildSelectQuery(this, condition);
            return await Connection.QueryAsync<Player>(
              sql,
              param: param,
              transaction: Transaction
            );
        }

        public override async Task<IEnumerable<Player>> FindLastPlayers(int amount, bool includeItems)
        {
            return await FindByCondition($"LIMIT {amount}", null, includeItems);
        }

        public override async Task<IEnumerable<Player>> FindAllBySteamIDs(IEnumerable<ulong> steamIDs, bool includeItems)
        {
            return await FindByCondition($"WHERE {nameof(Player.SteamID)} IN ({string.Join(",", steamIDs)})", null, includeItems);
        }
    }
}
