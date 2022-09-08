using Dapper;
using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class MatchesRepository : MatchesRepositoryBase
    {

        public override string TableName => "matches";

        private readonly MatchesPlayersRepositoryBase _matchPlayerRepository;

        public MatchesRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder, MatchesPlayersRepositoryBase matchPlayerRepository) : base(unitOfWork, queryBuilder)
        {
            _matchPlayerRepository = matchPlayerRepository;
        }

        public override async Task<Match?> FindByMatchID(ulong matchID, bool includePlayers)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(Match.ID), matchID);
            var sql = string.Empty;
            var condition = $"WHERE t1.{nameof(Match.ID)} = @{nameof(Match.ID)}";
            if (includePlayers)
            {
                sql = QueryBuilder.BuildJoinQuery(
                    new ComplexQueryTable<Match>(ComplexQueryTableJoinType.NotUsedInJoin, this),
                    new ComplexQueryTable<MatchPlayer>(ComplexQueryTableJoinType.LeftJoin, _matchPlayerRepository, $"ON t1.{nameof(Match.ID)} = t2.{nameof(MatchPlayer.MatchID)}"),
                    condition
                );
                var matchesDictionary = new Dictionary<ulong, Match>();
                var matchPlayers = new List<MatchPlayer>();
                await Connection.QueryAsync<Match, MatchPlayer, Match>(
                    sql,
                    (match, matchPlayer) =>
                    {
                        Match? matchEntry;

                        if (!matchesDictionary.TryGetValue(match.ID, out matchEntry))
                        {
                            matchEntry = match;
                            matchesDictionary.Add(matchEntry.ID, match);
                        }
                        if (matchPlayer != null)
                        {
                            matchPlayers.Add(matchPlayer);
                        }
                        return match;
                    },
                    param: parameters,
                    transaction: Transaction
                );
                var match = matchesDictionary.Values.FirstOrDefault();
                if(match != null)
                {
                    foreach (var matchPlayer in matchPlayers)
                    {
                        match.Players.Add(matchPlayer);
                    }
                }
                return match;

            }
            sql = QueryBuilder.BuildSelectQuery(this, condition);
            return await Connection.QuerySingleAsync<Match>(
              sql,
              param: parameters,
              transaction: Transaction
            );
        }

        public async override Task<Match?> FindByDOTAMatchID(ulong dotaMatchID, bool includePlayers)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(Match.DotaMatchID), dotaMatchID);
            var sql = string.Empty;
            var condition = $"WHERE t1.{nameof(Match.DotaMatchID)} = @{nameof(Match.DotaMatchID)}";
            if (includePlayers)
            {
                sql = QueryBuilder.BuildJoinQuery(
                    new ComplexQueryTable<Match>(ComplexQueryTableJoinType.NotUsedInJoin, this),
                    new ComplexQueryTable<MatchPlayer>(ComplexQueryTableJoinType.LeftJoin, _matchPlayerRepository, $"ON t1.{nameof(Match.ID)} = t2.{nameof(MatchPlayer.MatchID)}"),
                    condition
                );
                var matchesDictionary = new Dictionary<ulong, Match>();
                await Connection.QueryAsync<Match, MatchPlayer, Match>(
                    sql,
                    (match, matchPlayer) =>
                    {
                        Match? matchEntry;

                        if (!matchesDictionary.TryGetValue(match.ID, out matchEntry))
                        {
                            matchEntry = match;
                            matchesDictionary.Add(matchEntry.ID, match);
                        }
                        if (matchPlayer != null)
                        {
                            match.Players.Add(matchPlayer);
                        }
                        return match;
                    },
                    param: parameters,
                    transaction: Transaction
                );
                return matchesDictionary.Values.FirstOrDefault();

            }
            sql = QueryBuilder.BuildSelectQuery(this, condition);
            return await Connection.QuerySingleAsync<Match>(
              sql,
              param: parameters,
              transaction: Transaction
            );
        }
    }
}
