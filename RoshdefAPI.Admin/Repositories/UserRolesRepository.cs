using Dapper;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Admin.Repositories
{
    public class UsersRolesRepository : UsersRolesRepositoryBase
    {
        public override string TableName => "users_roles";

        public UsersRolesRepository(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public override async Task<UserRole> FindByNormalizedName(string normalizedName)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserRole.NormalizedName), normalizedName);
            return await Connection.QuerySingleOrDefaultAsync<UserRole>(
                QueryBuilder.BuildSelectQuery(this, $"WHERE t1.{nameof(UserRole.NormalizedName)} = @{nameof(UserRole.NormalizedName)}"),
                param: parameters,
                transaction: Transaction
            );
        }
    }
}
