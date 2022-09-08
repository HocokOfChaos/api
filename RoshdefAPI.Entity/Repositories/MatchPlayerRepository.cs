using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class MatchPlayerRepository : MatchesPlayersRepositoryBase
    {

        public override string TableName => "matches_players";

        public MatchPlayerRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder) { }
    }
}
