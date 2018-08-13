using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Website;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class UserLookup : IUserLookup
    {
        private readonly IEnumerable<IExternalUserSource> _externalUserSources;
        private readonly IGetNowForSite _getNowForSite;
        private readonly ISession _session;

        public UserLookup(ISession session, IEnumerable<IExternalUserSource> externalUserSources, IGetNowForSite getNowForSite)
        {
            _session = session;
            _externalUserSources = externalUserSources;
            // TODO: check if this is right for here, as users are system entities
            _getNowForSite = getNowForSite;
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
            var now = _getNowForSite.Now;
            return
                _session.QueryOver<User>()
                    .Where(
                        user =>
                            user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= now)
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
            return context.User != null && !string.IsNullOrWhiteSpace(context.User.Identity.Name)
                ? GetUserByEmail(context.User.Identity.Name)
                : null;
        }
    }
}