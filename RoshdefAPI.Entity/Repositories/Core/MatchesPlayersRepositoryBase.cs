using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class MatchesPlayersRepositoryBase : GenericRepository<MatchPlayer>
    {
        public MatchesPlayersRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }
    }
}
