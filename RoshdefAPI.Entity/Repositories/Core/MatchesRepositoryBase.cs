using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class MatchesRepositoryBase : GenericRepository<Match>
    {
        public MatchesRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task<Match?> FindByMatchID(ulong matchID, bool includePlayers);

        public abstract Task<Match?> FindByDOTAMatchID(ulong dotaMatchID, bool includePlayers);
    }
}
