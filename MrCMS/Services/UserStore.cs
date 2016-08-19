using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class UserStore : IUserLoginStore<User, int>, IUserClaimStore<User, int>, IUserRoleStore<User, int>
    {
        private readonly IRoleService _roleService;
        private readonly ISession _session;
        private readonly IUserLookup _userLookup;
        private readonly IUserManagementService _userManagementService;

        public UserStore(IUserManagementService userManagementService, IRoleService roleService, ISession session, IUserLookup userLookup)
        {
            _userManagementService = userManagementService;
            _session = session;
            _userLookup = userLookup;
            _roleService = roleService;
        }

        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
            IList<Claim> list = user.UserClaims.Select(claim => new Claim(claim.Claim, claim.Value)).ToList();
            return Task.FromResult(list);
        }

        public Task AddClaimAsync(User user, Claim claim)
        {
            var userClaim = new UserClaim
            {
                Claim = claim.Type,
                Value = claim.Value,
                Issuer = claim.Issuer,
                User = user
            };
            user.UserClaims.Add(userClaim);
            _session.Transact(session =>
            {
                session.Save(userClaim);
                session.Update(user);
            });
            return Task.FromResult<object>(null);
        }

        public Task RemoveClaimAsync(User user, Claim claim)
        {
            UserClaim singleOrDefault =
                user.UserClaims.SingleOrDefault(
                    userClaim =>
                        userClaim.Claim == claim.Type && userClaim.Value == claim.Value &&
                        userClaim.Issuer == claim.Issuer);
            if (singleOrDefault != null)
                _session.Transact(session =>
                {
                    user.UserClaims.Remove(singleOrDefault);
                    session.Delete(singleOrDefault);
                    session.Update(user);
                });
            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
        }

        public Task CreateAsync(User user)
        {
            _userManagementService.AddUser(user);
            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(User user)
        {
            _userManagementService.SaveUser(user);
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(User user)
        {
            _userManagementService.DeleteUser(user);
            return Task.FromResult<object>(null);
        }

        public Task<User> FindByIdAsync(int userId)
        {
            var user = _userManagementService.GetUser(userId);
            return Task.FromResult(user);
        }

        public Task<User> FindByNameAsync(string userName)
        {
            var user = _userLookup.GetUserByEmail(userName);
            return Task.FromResult(user);
        }

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            var userLogin = new UserLogin
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                User = user
            };
            user.UserLogins.Add(userLogin);
            _session.Transact(session =>
            {
                session.Save(userLogin);
                session.Update(user);
            });
            return Task.FromResult<object>(null);
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            UserLogin userLogin =
                user.UserLogins.FirstOrDefault(l => l.ProviderKey == login.ProviderKey);
            if (userLogin != null)
                _session.Transact(session =>
                {
                    user.UserLogins.Remove(userLogin);
                    session.Delete(userLogin);
                    session.Update(user);
                });
            return Task.FromResult<object>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            IList<UserLoginInfo> list =
                user.UserLogins.Select(login => new UserLoginInfo(login.LoginProvider, login.ProviderKey)).ToList();
            return Task.FromResult(list);
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            UserLogin singleOrDefault =
                _session.QueryOver<UserLogin>()
                    .Where(
                        userLogin =>
                            userLogin.ProviderKey == login.ProviderKey &&
                            userLogin.LoginProvider == login.LoginProvider)
                    .SingleOrDefault();

            return Task.FromResult(singleOrDefault != null ? singleOrDefault.User : null);
        }

        public Task AddToRoleAsync(User user, string role)
        {
            UserRole userRole = _roleService.GetRoleByName(role);
            if (userRole != null)
            {
                if (!user.Roles.Contains(userRole))
                    user.Roles.Add(userRole);
                if (!userRole.Users.Contains(user))
                    userRole.Users.Add(user);

                _userManagementService.SaveUser(user);
                _roleService.SaveRole(userRole);
            }
            return Task.FromResult<object>(null);
        }

        public Task RemoveFromRoleAsync(User user, string role)
        {
            UserRole userRole = _roleService.GetRoleByName(role);
            if (userRole != null)
            {
                if (user.Roles.Contains(userRole))
                    user.Roles.Remove(userRole);
                if (userRole.Users.Contains(user))
                    userRole.Users.Remove(user);

                _userManagementService.SaveUser(user);
                _roleService.SaveRole(userRole);
            }
            return Task.FromResult<object>(null);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            IList<string> roles =
                _roleService.GetAllRoles().Select(role => role.Name).ToList();
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(User user, string role)
        {
            return Task.FromResult(user.Roles.Any(
                 userRole =>
                     userRole.Name.Equals(role, StringComparison.OrdinalIgnoreCase)));
        }
    }
}