using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class UserLookup : IUserLookup
    {
        private readonly IGlobalRepository<User> _repository;
        private readonly IEnumerable<IExternalUserSource> _externalUserSources;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public UserLookup(IGlobalRepository<User> repository, IEnumerable<IExternalUserSource> externalUserSources, IGetDateTimeNow getDateTimeNow)
        {
            _repository = repository;
            _externalUserSources = externalUserSources;
            _getDateTimeNow = getDateTimeNow;
        }

        public User GetUserByEmail(string email)
        {
            var trimmedEmail = (email ?? string.Empty).Trim();
            var user =
                _repository.Query().FirstOrDefault(u => u.Email == trimmedEmail);
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
                _repository.Query()
                    .FirstOrDefault(
                        user =>
                            user.ResetPasswordGuid == resetGuid && user.ResetPasswordExpiry >= _getDateTimeNow.UtcNow);
        }

        public User GetUserByGuid(Guid guid)
        {
            return
                _repository.Query()
                    .FirstOrDefault(user => user.Guid == guid);
        }

        public User GetUserById(int id)
        {
            return _repository.LoadSync(id);
        }

        public User GetCurrentUser(HttpContext context)
        {
            return GetCurrentUser(context?.User);
        }

        public User GetCurrentUser(IPrincipal principal)
        {
            return principal!= null && !string.IsNullOrWhiteSpace(principal.Identity.Name)
                ? GetUserByEmail(principal.Identity.Name)
                : null;
        }
    }
}