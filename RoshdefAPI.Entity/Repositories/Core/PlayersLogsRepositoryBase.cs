using RoshdefAPI.Data.Models;
using RoshdefAPI.Data.Models.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories.Core
{
    public abstract class PlayersLogsRepositoryBase : GenericRepository<PlayerLog>
    {
        public PlayersLogsRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task Log(ulong steamID, IPlayerLogEntry logEntry);

        public abstract Task<IEnumerable<PlayerLog>> FindLogsBySteamID(ulong steamID);
    }
}
