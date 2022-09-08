using RoshdefAPI.Admin.Models;
using RoshdefAPI.Entity.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;

namespace RoshdefAPI.Admin.Repositories.Core
{
    public abstract class UsersRepositoryBase : GenericRepository<User>
    {
        public UsersRepositoryBase(IUnitOfWork unitOfWork, IQueryBuilder queryBuilder) : base(unitOfWork, queryBuilder)
        {
        }

        public abstract Task<User> FindByNormalizedLogin(string normalizedName);
    }
}
