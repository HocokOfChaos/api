using RoshdefAPI.Data.Models;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class PlayersRepositoryBase : GenericRepository<Player>
    {
        public PlayersRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task<Player?> FindBySteamID(ulong steamID, bool includeItems);
        public abstract Task<Player?> Find(ulong id, bool includeItems);
        public abstract Task<IEnumerable<Player>> FindAll(bool includeItems);
        public abstract Task<IEnumerable<Player>> FindLastPlayers(int amount, bool includeItems);
        public abstract Task<IEnumerable<Player>> FindAllBySteamIDs(IEnumerable<ulong> steamIDs, bool includeItems);
    }
}
