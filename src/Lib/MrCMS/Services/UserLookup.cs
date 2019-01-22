using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class UserLookup : IUserLookup
    {
        private readonly IEnumerable<IExternalUserSource> _externalUserSources;
        private readonly ISession _session;

        public UserLookup(ISession session, IEnumerable<IExternalUserSource> externalUserSources)
        {
            _session = session;
            _externalUserSources = externalUserSources;
        }

        public User GetUserByEmail(string email)
        {
            var trimmedEmail = (email ?? string.Empty).Trim();
            var user =
                _session.QueryOver<User>().Where(u => u.Email == trimmedEmail).Take(1).Cacheable().SingleOrDefault();
            if (user != null)
                return user;

            foreach (var source in _externalUserSources)
            {
                user = source.SynchroniseUser(email);
                if (user != null)
                    return user;
            }

            return null;
        }

        public User GetUserByResetGuid(Guid resetGuid)
        {
            return
                _session.QueryOver<User>()
                    .Where(
                        user =>
                            user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= DateTime.UtcNow)
                    .Cacheable().SingleOrDefault();
        }

        public User GetUserByGuid(Guid guid)
        {
            return _session.QueryOver<User>()
                .Where(user => user.Guid == guid)
                .Cacheable().SingleOrDefault();
        }

        public User GetCurrentUser(HttpContext context)
        {
            return GetCurrentUser(context?.User);
        }

        public User GetCurrentUser(IPrincipal principal)
        {
            return principal != null && !string.IsNullOrWhiteSpace(principal.Identity.Name)
                ? GetUserByEmail(principal.Identity.Name)
                : null;
        }
    }
}