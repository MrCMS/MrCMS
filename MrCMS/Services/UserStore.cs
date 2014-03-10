using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class UserStore : IUserLoginStore<User>, IUserClaimStore<User>, IUserRoleStore<User>
    {
        private readonly IRoleService _roleService;
        private readonly ISession _session;
        private readonly IUserService _userService;

        public UserStore(IUserService userService, IRoleService roleService, ISession session)
        {
            _userService = userService;
            _session = session;
            _roleService = roleService;
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await Task.Factory.StartNew(() =>
                                               {
                                                   IList<Claim> list = new List<Claim>();
                                                   foreach (UserClaim claim in user.UserClaims)
                                                       list.Add(new Claim(claim.Claim, claim.Value));
                                                   return list;
                                               }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task AddClaimAsync(User user, Claim claim)
        {
            await Task.Factory.StartNew(() =>
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
                                        }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task RemoveClaimAsync(User user, Claim claim)
        {
            await Task.Factory.StartNew(() =>
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
                                        }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void Dispose()
        {
        }

        public async Task CreateAsync(User user)
        {
            await Task.Factory.StartNew(() => _userService.AddUser(user), CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task UpdateAsync(User user)
        {
            await Task.Factory.StartNew(() => _userService.SaveUser(user), CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task DeleteAsync(User user)
        {
            await Task.Factory.StartNew(() => _userService.DeleteUser(user), CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            return await Task.Factory.StartNew(() =>
                                               {
                                                   int id;
                                                   int.TryParse(userId, out id);
                                                   return _userService.GetUser(id);
                                               }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<User> FindByNameAsync(string userName)
        {
            return await Task.Factory.StartNew(() =>
                _userService.GetUserByEmail(userName), CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task AddLoginAsync(User user, UserLoginInfo login)
        {
            await Task.Factory.StartNew(() =>
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
                                        }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            await Task.Factory.StartNew(() =>
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
                                        }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            return await Task.Factory.StartNew(() =>
                                               {
                                                   IList<UserLoginInfo> list = new List<UserLoginInfo>();
                                                   foreach (UserLogin login in user.UserLogins)
                                                       list.Add(new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                                                   return list;
                                               }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<User> FindAsync(UserLoginInfo login)
        {
            return await Task.Factory.StartNew(() =>
                                               {
                                                   UserLogin singleOrDefault =
                                                       _session.QueryOver<UserLogin>()
                                                           .Where(
                                                               userLogin =>
                                                                   userLogin.ProviderKey == login.ProviderKey &&
                                                                   userLogin.LoginProvider == login.LoginProvider)
                                                           .SingleOrDefault();

                                                   return singleOrDefault != null ? singleOrDefault.User : null;
                                               }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            await Task.Factory.StartNew(() =>
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
                                        }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task RemoveFromRoleAsync(User user, string role)
        {
            await Task.Factory.StartNew(() =>
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
                                        }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await Task.Factory.StartNew(() =>
                                               {
                                                   IList<string> roles =
                                                       _roleService.GetAllRoles().Select(role => role.Name).ToList();
                                                   return roles;
                                               }, CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await Task.Factory.StartNew(() =>
                user.Roles.Any(
                    userRole =>
                        userRole.Name.Equals(role, StringComparison.OrdinalIgnoreCase)),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}