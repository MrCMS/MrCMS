using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;
using System.Linq;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class UserStore : IUserLoginStore<User>, IUserClaimStore<User>, IUserRoleStore<User>
    {
        private readonly IUserService _userService;
        private readonly ISession _session;
        private readonly IRoleService _roleService;

        public UserStore(IUserService userService, IRoleService roleService, ISession session)
        {
            _userService = userService;
            _session = session;
            _roleService = roleService;
        }

        public void Dispose()
        {
        }

        public Task CreateAsync(User user)
        {
            return Task.Run(() => _userService.AddUser(user));
        }

        public Task UpdateAsync(User user)
        {
            return Task.Run(() => _userService.SaveUser(user));
        }

        public Task DeleteAsync(User user)
        {
            return Task.Run(() => _userService.DeleteUser(user));
        }

        public Task<User> FindByIdAsync(string userId)
        {
            return Task.Run(() =>
                {
                    int id;
                    int.TryParse(userId, out id);
                    return _userService.GetUser(id);
                });
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return Task.Run(() => _userService.GetUserByEmail(userName));
        }

        public Task AddLoginAsync(User user, UserLoginInfo login)
        {
            return Task.Run(() =>
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
                });
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            return Task.Run(() =>
                {
                    UserLogin userLogin = user.UserLogins.FirstOrDefault(l => l.ProviderKey == login.ProviderKey);
                    if (userLogin != null)
                        _session.Transact(session =>
                            {
                                user.UserLogins.Remove(userLogin);
                                session.Delete(userLogin);
                                session.Update(user);
                            });
                });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            return Task.Run(() =>
                {
                    IList<UserLoginInfo> list = new List<UserLoginInfo>();
                    foreach (UserLogin login in user.UserLogins)
                        list.Add(new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                    return list;
                });
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            return Task.Run(() =>
                {
                    UserLogin singleOrDefault =
                        _session.QueryOver<UserLogin>()
                                .Where(
                                    userLogin =>
                                    userLogin.ProviderKey == login.ProviderKey &&
                                    userLogin.LoginProvider == login.LoginProvider)
                                .SingleOrDefault();

                    return singleOrDefault != null ? singleOrDefault.User : null;
                });
        }

        public Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return Task.Run(() =>
                {
                    IList<Claim> list = new List<Claim>();
                    foreach (UserClaim claim in user.UserClaims)
                        list.Add(new Claim(claim.Claim, claim.Value));
                    return list;
                });
        }

        public Task AddClaimAsync(User user, Claim claim)
        {
            return Task.Run(() =>
                {
                    var userClaim = new UserClaim { Claim = claim.Type, Value = claim.Value, Issuer = claim.Issuer, User = user };
                    user.UserClaims.Add(userClaim);
                    _session.Transact(session =>
                        {
                            session.Save(userClaim);
                            session.Update(user);
                        });
                });
        }

        public Task RemoveClaimAsync(User user, Claim claim)
        {
            return Task.Run(() =>
                {
                    UserClaim singleOrDefault = user.UserClaims.SingleOrDefault(userClaim => userClaim.Claim == claim.Type && userClaim.Value == claim.Value && userClaim.Issuer == claim.Issuer);
                    if (singleOrDefault != null)
                        _session.Transact(session =>
                            {
                                user.UserClaims.Remove(singleOrDefault);
                                session.Delete(singleOrDefault);
                                session.Update(user);
                            });
                });
        }

        public Task AddToRoleAsync(User user, string role)
        {
            return Task.Run(() =>
                {
                    UserRole userRole = _roleService.GetRoleByName(role);
                    if (userRole != null)
                    {
                        if (!user.Roles.Contains(userRole))
                            user.Roles.Add(userRole);
                        if (!userRole.Users.Contains(user))
                            userRole.Users.Add(user);

                        _userService.SaveUser(user);
                        _roleService.SaveRole(userRole);
                    }
                });
        }

        public Task RemoveFromRoleAsync(User user, string role)
        {
            return Task.Run(() =>
                {
                    UserRole userRole = _roleService.GetRoleByName(role);
                    if (userRole != null)
                    {
                        if (user.Roles.Contains(userRole))
                            user.Roles.Remove(userRole);
                        if (userRole.Users.Contains(user))
                            userRole.Users.Remove(user);

                        _userService.SaveUser(user);
                        _roleService.SaveRole(userRole);
                    }
                });
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            return Task.Run(() =>
                {
                    IList<string> roles = _roleService.GetAllRoles().Select(role => role.Name).ToList();
                    return roles;
                });
        }

        public Task<bool> IsInRoleAsync(User user, string role)
        {
            return
                Task.Run(
                    () => user.Roles.Any(userRole => userRole.Name.Equals(role, StringComparison.OrdinalIgnoreCase)));
        }
    }
}