using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Website;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class UserLookup : IUserLookup
    {
        private readonly IEnumerable<IExternalUserSource> _externalUserSources;
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly ISession _session;

        public UserLookup(ISession session, IEnumerable<IExternalUserSource> externalUserSources,
            IGetDateTimeNow getDateTimeNow)
        {
            _session = session;
            _externalUserSources = externalUserSources;
            _getDateTimeNow = getDateTimeNow;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var trimmedEmail = (email ?? string.Empty).Trim();
            var user =
                await _session.QueryOver<User>().Where(u => u.Email == trimmedEmail).Take(1).Cacheable()
                    .SingleOrDefaultAsync();
            if (user != null)
                return user;

            //todo is this still required? Test external user auth with aspnetcore.Identity
            foreach (var source in _externalUserSources)
            {
                user = await source.SynchroniseUser(email);
                if (user != null)
                    return user;
            }

            return null;
        }

        //todo is this still required?
        public async Task<User> GetUserByResetGuid(Guid resetGuid)
        {
            return await
                _session.QueryOver<User>()
                    .Where(
                        user =>
                            user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= _getDateTimeNow.UtcNow)
                    .Cacheable().SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByGuid(Guid guid)
        {
            return await _session.QueryOver<User>()
                .Where(user => user.Guid == guid)
                .Cacheable().SingleOrDefaultAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _session.GetAsync<User>(id);
        }

        public async Task<User> GetCurrentUser(HttpContext context)
        {
            return await GetCurrentUser(context?.User);
        }

        public async Task<User> GetCurrentUser(IPrincipal principal)
        {
            return principal != null && !string.IsNullOrWhiteSpace(principal.Identity.Name)
                ? await GetUserByEmail(principal.Identity.Name)
                : null;
        }
    }
}