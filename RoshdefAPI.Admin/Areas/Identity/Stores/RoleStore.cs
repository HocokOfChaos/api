using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Shared.Models.Configuration;

namespace RoshdefAPI.Areas.Identify.Stores
{
    public class RoleStore : IRoleStore<UserRole>
    {
        private readonly ILogger<RoleStore> _logger;
        private readonly UsersRolesRepositoryBase _usersRolesRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationSettings _config;
        public RoleStore(ILogger<RoleStore> logger, UsersRolesRepositoryBase userRolesRepository, IUnitOfWork unitOfWork, IOptions<ApplicationSettings> config)
        {
            _logger = logger;
            _usersRolesRepository = userRolesRepository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
        }

        public async Task<IdentityResult> CreateAsync(UserRole role, CancellationToken cancellationToken)
        {
            if (!_config.RegistrationEnabled)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Registration disabled." });
            }
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);
            try
            {
                await _usersRolesRepository.Insert(role);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateAsync failed.", ex);
                return IdentityResult.Failed(new IdentityError { Description = $"Could not create role with {nameof(role.Name)} = {role.Name} ({role.ID})." });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(UserRole role, CancellationToken cancellationToken)
        {
            if (!_config.RegistrationEnabled)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Registration disabled." });
            }
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);
            try
            {
                await _usersRolesRepository.Delete(role);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteAsync failed.", ex);
                return IdentityResult.Failed(new IdentityError { Description = $"Could not delete role with {nameof(role.Name)} = {role.Name} ({role.ID})." });
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<UserRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(roleId);
            if (!ulong.TryParse(roleId, out ulong id))
            {
                var ex = new ArgumentException($"Not a valid {nameof(roleId)}", nameof(roleId));
                _logger.LogError("FindByIdAsync failed.", ex);
                throw ex;
            }
            UserRole userRole;
            try
            {
                userRole = await _usersRolesRepository.Find(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("FindByIdAsync failed.", ex);
                return null;
            }
            return userRole;
        }

        public async Task<UserRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(normalizedRoleName);
            UserRole userRole;
            try
            {
                userRole = await _usersRolesRepository.FindByNormalizedName(normalizedRoleName);
            }
            catch (Exception ex)
            {
                _logger.LogError("FindByNameAsync failed.", ex);
                return null;
            }
            return userRole;
        }

        public Task<string> GetNormalizedRoleNameAsync(UserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(UserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);

            return Task.FromResult(role.ID.ToString());
        }

        public Task<string> GetRoleNameAsync(UserRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);

            return Task.FromResult(role.Name.ToString());
        }

        public Task SetNormalizedRoleNameAsync(UserRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);
            ArgumentNullException.ThrowIfNull(normalizedName);

            role.NormalizedName = normalizedName;
            return Task.FromResult<object>(null);
        }

        public Task SetRoleNameAsync(UserRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);
            ArgumentNullException.ThrowIfNull(roleName);
            role.Name = roleName;

            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> UpdateAsync(UserRole role, CancellationToken cancellationToken)
        {
            if (!_config.RegistrationEnabled)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Registration disabled." });
            }
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(role);
            try
            {
                await _usersRolesRepository.Update(role);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateAsync failed.", ex);
                return IdentityResult.Failed(new IdentityError { Description = $"Could not update user with {nameof(role.Name)} = {role.Name} ({role.ID})." });
            }

            return IdentityResult.Success;
        }
    }
}
