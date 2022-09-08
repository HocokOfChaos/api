using RoshdefAPI.Admin.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Admin.Repositories.Core
{
    public abstract class UsersRolesRepositoryBase : GenericRepository<UserRole>
    {
        public UsersRolesRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task<UserRole> FindByNormalizedName(string normalizedName);
    }
}
