using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class PlayersItemsRepositoryBase : GenericRepository<PlayerItem>
    {
        public PlayersItemsRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }
    }
}
