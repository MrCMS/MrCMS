using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services
{
    public class UserLookup : IUserLookup
    {
        private readonly IRepository<User> _userRepository;
        private readonly IEnumerable<IExternalUserSource> _externalUserSources;
        private readonly IGetNow _getNow;

        public UserLookup(IRepository<User> userRepository, IEnumerable<IExternalUserSource> externalUserSources, IGetNow getNow)
        {
            _userRepository = userRepository;
            _externalUserSources = externalUserSources;
            _getNow = getNow;
        }

        public User GetUserByEmail(string email)
        {
            var trimmedEmail = (email ?? string.Empty).Trim();
            var user =
                _userRepository.Query().FirstOrDefault(u => u.Email == trimmedEmail);
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
                _userRepository.Query()
                    .FirstOrDefault(
                        user =>
                            user.ResetPasswordGuid == resetGuid &&
                            user.ResetPasswordExpiry >= _getNow.Get());
        }

        public User GetCurrentUser(HttpContextBase context)
        {
            return context.User != null && !string.IsNullOrWhiteSpace(context.User.Identity.Name)
                ? GetUserByEmail(context.User.Identity.Name)
                : null;
        }
    }
}