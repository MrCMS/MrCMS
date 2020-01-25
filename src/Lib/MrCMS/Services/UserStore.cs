using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;

namespace MrCMS.Services
{
    public class UserStore : IUserClaimStore<User>,
        IUserEmailStore<User>,
        IUserRoleStore<User>,
        //IUserPasswordStore<User>,
        // TODO: implement password store
        //IUserLockoutStore<User>, 
        // TODO: implement lockout store properly
        //IUserPhoneNumberStore<User>,
        // TODO: implement phone number store
        IQueryableUserStore<User>,
        IUserLoginStore<User>
    {
        private readonly IGlobalRepository<User> _repository;
        private readonly IGlobalRepository<UserClaim> _userClaimRepository;
        private readonly IGlobalRepository<UserLogin> _userLoginRepository;
        private readonly IGlobalRepository<Role> _roleRepository;
        private readonly IGlobalRepository<UserToRole> _userToRoleRepository;

        public UserStore(
            IGlobalRepository<User> repository,
            IGlobalRepository<UserClaim> userClaimRepository,
            IGlobalRepository<UserLogin> userLoginRepository,
            IGlobalRepository<Role> roleRepository,
            IGlobalRepository<UserToRole> userToRoleRepository
            )
        {
            _repository = repository;
            _userClaimRepository = userClaimRepository;
            _userLoginRepository = userLoginRepository;
            _roleRepository = roleRepository;
            _userToRoleRepository = userToRoleRepository;
        }

        public void Dispose()
        {
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Email);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Email);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.Add(user, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.Update(user, cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _repository.Delete(user, cancellationToken);
            return IdentityResult.Success;
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return int.TryParse(userId, out int id)
                ? _repository.Load(id, cancellationToken)
                : Task.FromResult<User>(null);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return _repository.Query()
                .FirstOrDefaultAsync(x => x.Email == normalizedUserName, cancellationToken);
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken cancellationToken)
        {
            var userClaims = await GetUserClaimsAsync(user, cancellationToken);

            return userClaims.Select(claim => new Claim(claim.Claim, claim.Value, null, claim.Issuer)).ToList();
        }

        private async Task<List<UserClaim>> GetUserClaimsAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                return new List<UserClaim>();
            }

            var userClaims = await _userClaimRepository.Query().Where(x => x.UserId == user.Id)
                //.WithOptions(options => options.SetCacheable(true))
                .ToListAsync(cancellationToken);
            return userClaims;
        }

        public async Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var userClaims = claims.Select(claim => MapUserClaim(claim, user));
            await _repository.Transact(async (repo, ct) =>
            {
                foreach (var userClaim in userClaims)
                {
                    user.UserClaims.Add(userClaim);
                }

                await _userClaimRepository.AddRange(userClaims.ToList(), ct);
                await repo.Update(user, ct);
            }, cancellationToken);
        }

        private static UserClaim MapUserClaim(Claim claim, User user)
        {
            return new UserClaim
            {
                Claim = claim.Type,
                Value = claim.Value,
                Issuer = claim.Issuer,
                User = user
            };
        }

        public async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            var existingClaims = await GetUserClaimsAsync(user, cancellationToken);
            var existingUserClaim = existingClaims.FirstOrDefault(x => x.Claim == claim.Type);
            var newUserClaim = MapUserClaim(newClaim, user);

            await _repository.Transact(async (repo, ct) =>
            {
                await _userClaimRepository.Delete(existingUserClaim, ct);
                user.UserClaims.Remove(existingUserClaim);

                await _userClaimRepository.Add(newUserClaim, ct);
                user.UserClaims.Add(newUserClaim);

                await repo.Update(user, ct);
            }, cancellationToken);

        }

        public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var existingClaims = await GetUserClaimsAsync(user, cancellationToken);
            await _repository.Transact(async (repo, ct) =>
            {
                foreach (var claim in claims)
                {
                    var existingUserClaim = existingClaims.FirstOrDefault(x => x.Claim == claim.Type);
                    if (existingUserClaim == null)
                    {
                        continue;
                    }

                    await _userClaimRepository.Delete(existingUserClaim, ct);
                    user.UserClaims.Remove(existingUserClaim);
                }

                await repo.Update(user, ct);
            }, cancellationToken);
        }

        public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return await _userClaimRepository.Query<UserClaim>()
                .Where(x => x.Claim == claim.Type && x.Value == claim.Value)
                .Include(x => x.User)
                .Select(x => x.User)
                .ToListAsync(cancellationToken);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return UpdateAsync(user, cancellationToken);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            // TODO: implement email confirmed
            return Task.FromResult(true);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            // TODO: implement email confirmed
            return Task.CompletedTask;
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _repository.Query()
                .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Email);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await GetRoleByName(roleName, cancellationToken);
            if (role != null)
            {
                var userToRole = new UserToRole { UserRoleId = role.Id, UserId = user.Id };

                await _userToRoleRepository.Add(userToRole);
            }
        }

        private async Task<Role> GetRoleByName(string roleName, CancellationToken cancellationToken)
        {
            if (roleName == null)
                return null;
            return await _roleRepository.Query()
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == roleName.ToUpper(), cancellationToken);
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
                return;
            var role = await GetRoleByName(roleName, cancellationToken);
            if (role != null)
            {
                var existingRoleMapping = await _userToRoleRepository.Query()
                    .FirstOrDefaultAsync(x => x.UserId == user.Id && x.UserRoleId == role.Id, cancellationToken);
                if (existingRoleMapping != null)
                {
                    await _userToRoleRepository.Delete(existingRoleMapping, cancellationToken);
                }
            }

        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            return await _userToRoleRepository.Readonly().Where(x => x.UserId == user.Id).Select(x => x.UserRole.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            return await _userToRoleRepository.Readonly()
                .AnyAsync(x => x.UserId == user.Id && x.UserRole.Name.ToUpper() == roleName.ToUpper(), cancellationToken);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _userToRoleRepository.Query().Where(x => x.UserRole.Name.ToUpper() == roleName.ToUpper()).Select(x => x.User)
                           .ToListAsync(cancellationToken);
        }

        //public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        //{
        //    user.PasswordHash 
        //    throw new NotImplementedException();
        //}

        //public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task ResetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> GetAccessFailedCountAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> GetLockoutEnabledAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}

        public IQueryable<User> Users => _repository.Query();
        public async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            var userLogin = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                DisplayName = login.ProviderDisplayName,
                User = user
            };
            user.UserLogins.Add(userLogin);
            await _userLoginRepository.Transact(async (repo, token) =>
            {
                await repo.Add(userLogin, token);
                await _repository.Update(user, token);
            }, cancellationToken);

        }

        public async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            UserLogin userLogin =
                user.UserLogins.FirstOrDefault(l => l.ProviderKey == providerKey && l.LoginProvider == loginProvider);
            if (userLogin != null)
            {
                await _userLoginRepository.Transact(async (repo, token) =>
                {
                    user.UserLogins.Remove(userLogin);
                    await repo.Delete(userLogin, token);
                    await _repository.Update(user, token);
                }, cancellationToken);
            }
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            IList<UserLoginInfo> list =
                user.UserLogins.Select(login =>
                        new UserLoginInfo(login.LoginProvider, login.ProviderKey, login.DisplayName ?? user.Email))
                    .ToList();
            return Task.FromResult(list);
        }

        public Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return _userLoginRepository.Query()
                    .Where(
                        userLogin =>
                            userLogin.ProviderKey == providerKey &&
                            userLogin.LoginProvider == loginProvider)
                    .Select(x => x.User)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}