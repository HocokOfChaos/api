using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RoshdefAPI.Admin.Models;
using RoshdefAPI.Admin.Repositories.Core;
using RoshdefAPI.Entity.Services.Core;
using RoshdefAPI.Shared.Models.Configuration;

namespace RoshdefAPI.Areas.Identify.Stores
{
    public class UserStore : IUserStore<User>,
        IUserPasswordStore<User>
    {
        private readonly ILogger<UserStore> _logger;
        private readonly UsersRepositoryBase _usersRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationSettings _config;

        public UserStore(ILogger<UserStore> logger, UsersRepositoryBase userRepository, IUnitOfWork unitOfWork, IOptions<ApplicationSettings> config)
        {
            _logger = logger;
            _usersRepository = userRepository;
            _unitOfWork = unitOfWork;
            _config = config.Value;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            if(!_config.RegistrationEnabled)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Registration disabled." });
            }
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                await _usersRepository.Insert(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateAsync failed.", ex);
                return IdentityResult.Failed(new IdentityError { Description = $"Could not create user with {nameof(user.Login)} = {user.Login} ({user.ID})." });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            if (!_config.RegistrationEnabled)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Registration disabled." });
            }
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                await _usersRepository.Delete(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteAsync failed.", ex);
                return IdentityResult.Failed(new IdentityError { Description = $"Could not delete user with {nameof(user.Login)} = {user.Login} ({user.ID})." });
            }

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(userId);
            if (!ulong.TryParse(userId, out ulong id))
            {
                var ex = new ArgumentException($"Not a valid {nameof(userId)}", nameof(userId));
                _logger.LogError("FindByIdAsync failed.", ex);
                throw ex;
            }
            User user;
            try
            {
                user = await _usersRepository.Find(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("FindByIdAsync failed.", ex);
                return null;
            }

            return user;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(normalizedUserName);
            User user;
            try
            {
                user = await _usersRepository.FindByNormalizedLogin(normalizedUserName);
            }
            catch (Exception ex)
            {
                _logger.LogError("FindByNameAsync failed.", ex);
                return null;
            }

            return user;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.NormalizedLogin);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.ID.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.Login.ToString());
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);

            return Task.FromResult(user.PasswordHash is not null);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(normalizedName);
            user.NormalizedLogin = normalizedName;

            return Task.FromResult<object>(null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(passwordHash);

            user.PasswordHash = passwordHash;
            return Task.FromResult<object>(null);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(userName);
            user.Login = userName;

            return Task.FromResult<object>(null);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if (!_config.RegistrationEnabled)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Registration disabled." });
            }
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentNullException.ThrowIfNull(user);
            try
            {
                await _usersRepository.Update(user);
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateAsync failed.", ex);
                return IdentityResult.Failed(new IdentityError { Description = $"Could not update user with {nameof(user.Login)} = {user.Login} ({user.ID})." });
            }

            return IdentityResult.Success;
        }
    }
}
