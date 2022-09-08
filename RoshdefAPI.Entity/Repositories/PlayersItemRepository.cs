using Dapper;
using RoshdefAPI.Data;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Entity.Repositories
{
    public class PlayersItemRepository : PlayersItemsRepositoryBase
    {

        public override string TableName => "players_items";

        public PlayersItemRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder) { }
    }
}
