using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly ISession _session;

        public UserStore(ISession session)
        {
            _session = session;
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
            // TODO: look to see if we want to be able to this
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Email);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            // TODO: look to see if we want to be able to this
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _session.TransactAsync((session, token) => session.SaveAsync(user, token), cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            await _session.TransactAsync((session, token) => session.UpdateAsync(user, token), cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            await _session.TransactAsync((session, token) => session.DeleteAsync(user, token), cancellationToken);
            return IdentityResult.Success;
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return int.TryParse(userId, out int id)
                ? _session.GetAsync<User>(id, cancellationToken)
                : Task.FromResult<User>(null);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return _session.Query<User>().WithOptions(x => x.SetCacheable(true))
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

            var userClaims = await _session.Query<UserClaim>().Where(x => x.User.Id == user.Id)
                .WithOptions(options => options.SetCacheable(true))
                .ToListAsync(cancellationToken);
            return userClaims;
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var userClaims = claims.Select(claim => MapUserClaim(claim, user));
            return _session.TransactAsync(async (session, token) =>
             {
                 foreach (var userClaim in userClaims)
                 {
                     await session.SaveAsync(userClaim, token);
                     user.UserClaims.Add(userClaim);
                 }
                 await session.UpdateAsync(user, token);
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

            await _session.TransactAsync(async (session, token) =>
            {
                await session.DeleteAsync(existingUserClaim, token);
                user.UserClaims.Remove(existingUserClaim);

                await session.SaveAsync(newUserClaim, token);
                user.UserClaims.Add(newUserClaim);

                await session.UpdateAsync(user, token);
            }, cancellationToken);

        }

        public async Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var existingClaims = await GetUserClaimsAsync(user, cancellationToken);
            await _session.TransactAsync(async (session, token) =>
            {
                foreach (var claim in claims)
                {
                    var existingUserClaim = existingClaims.FirstOrDefault(x => x.Claim == claim.Type);
                    if (existingUserClaim == null)
                    {
                        continue;
                    }

                    await session.DeleteAsync(existingUserClaim, token);
                    user.UserClaims.Remove(existingUserClaim);
                }
                await session.UpdateAsync(user, token);
            }, cancellationToken);
        }

        public async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return await _session.Query<UserClaim>()
                .Where(x => x.Claim == claim.Type && x.Value == claim.Value)
                .WithOptions(options => options.SetCacheable(true))
                .Fetch(x => x.User)
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
            return _session.Query<User>().WithOptions(x => x.SetCacheable(true))
                .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user?.Email);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            // TODO: implement normalised email
            return Task.CompletedTask;
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await GetRoleByName(roleName, cancellationToken);
            if (role != null)
            {
                if (!user.Roles.Contains(role))
                {
                    user.Roles.Add(role);
                }

                if (!role.Users.Contains(user))
                {
                    role.Users.Add(user);
                }

                await _session.TransactAsync(async (session, token) =>
                {
                    await session.UpdateAsync(user, cancellationToken);
                    await session.UpdateAsync(role, cancellationToken);
                }, cancellationToken);
            }
        }

        private async Task<UserRole> GetRoleByName(string roleName, CancellationToken cancellationToken)
        {
            if (roleName == null)
                return null;
            var role = await _session.Query<UserRole>().WithOptions(x => x.SetCacheable(true))
                .FirstOrDefaultAsync(x => x.Name.ToUpper() == roleName.ToUpper(), cancellationToken);
            return role;
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await GetRoleByName(roleName, cancellationToken);
            if (role != null)
            {
                if (user.Roles.Contains(role))
                {
                    user.Roles.Remove(role);
                }

                if (role.Users.Contains(user))
                {
                    role.Users.Remove(user);
                }

                await _session.TransactAsync(async (session, token) =>
                {
                    await session.UpdateAsync(user, cancellationToken);
                    await session.UpdateAsync(role, cancellationToken);
                }, cancellationToken);
            }

        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            return user.Roles.Select(x => x.Name).ToList();
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var role = await GetRoleByName(roleName, cancellationToken);
            return user.Roles.Contains(role);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await GetRoleByName(roleName, cancellationToken);
            return role.Users.ToList();
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

        public IQueryable<User> Users => _session.Query<User>().WithOptions(x => x.SetCacheable(true));
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
            await _session.TransactAsync(async (session, token) =>
            {
                await session.SaveAsync(userLogin, token);
                await session.UpdateAsync(user, token);
            }, cancellationToken);

        }

        public async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            UserLogin userLogin =
                user.UserLogins.FirstOrDefault(l => l.ProviderKey == providerKey && l.LoginProvider == loginProvider);
            if (userLogin != null)
            {
                await _session.TransactAsync(async (session, token) =>
                {
                    user.UserLogins.Remove(userLogin);
                    await session.DeleteAsync(userLogin, cancellationToken);
                    await session.UpdateAsync(user, cancellationToken);
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
            return _session.Query<UserLogin>()
                    .Where(
                        userLogin =>
                            userLogin.ProviderKey == providerKey &&
                            userLogin.LoginProvider == loginProvider)
                    .Select(x => x.User)
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}