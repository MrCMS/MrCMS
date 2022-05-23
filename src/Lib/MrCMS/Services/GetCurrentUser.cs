using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Website;
using MrCMS.Website.Auth;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserLookup _userLookup;
        private readonly ICacheInHttpContext _cacheInHttpContext;
        private readonly IAccessChecker _accessChecker;

        public GetCurrentUser(IHttpContextAccessor contextAccessor, IUserLookup userLookup,
            ICacheInHttpContext cacheInHttpContext, IAccessChecker accessChecker)
        {
            _contextAccessor = contextAccessor;
            _userLookup = userLookup;
            _cacheInHttpContext = cacheInHttpContext;
            _accessChecker = accessChecker;
        }

        public async Task<User> Get()
        {
            return await GetImpersonatedUser() ?? await GetLoggedInUser();
        }

        public async Task<User> GetLoggedInUser()
        {
            return await _cacheInHttpContext.GetForRequestAsync<User>("current.request.user",
                () => _userLookup.GetCurrentUser(_contextAccessor.HttpContext));
        }

        public async Task<User> GetImpersonatedUser()
        {
            var loggedInUser = await GetLoggedInUser();
            return await _cacheInHttpContext.GetForRequestAsync("current.request.user.impersonated", async () =>
            {
                // if they're not logged in, we're done
                if (loggedInUser == null)
                    return null;

                // otherwise, check if they can impersonate
                var canImpersonate = await _accessChecker.CanAccess<UserACL>(UserACL.Impersonate, loggedInUser);
                // if not, we're done
                if (!canImpersonate)
                    return null;

                // otherwise, check if they're impersonating someone
                var context = _contextAccessor.HttpContext;
                var impersonatingId = context!.Session.GetInt32(UserImpersonationService.UserImpersonationKey);
                if (!impersonatingId.HasValue)
                    return null;

                var impersonatedUser = await _userLookup.GetUserById(impersonatingId.Value);
                // if we can't find the user, we're done
                if (impersonatedUser == null)
                    return null;

                // cannot impersonate an admin
                if (impersonatedUser.IsAdmin)
                    return null;

                return impersonatedUser;
            });
        }
    }
}