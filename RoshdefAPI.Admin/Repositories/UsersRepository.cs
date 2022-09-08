using Dapper;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Admin.Repositories
{
    public class UsersRepository : UsersRepositoryBase
    {
        public override string TableName => "users";

        public UsersRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public override async Task<User> FindByNormalizedLogin(string normalizedName)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(User.NormalizedLogin), normalizedName);
            return await Connection.QuerySingleOrDefaultAsync<User>(
                QueryBuilder.BuildSelectQuery(this, $"WHERE t1.{nameof(User.NormalizedLogin)} = @{nameof(User.NormalizedLogin)}"),
                param: parameters,
                transaction: Transaction
            );
        }
    }
}
