using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserLookup _userLookup;
        private readonly ICacheInHttpContext _cacheInHttpContext;
        private readonly IUserImpersonationService _userImpersonationService;

        public GetCurrentUser(IHttpContextAccessor contextAccessor,
            IUserLookup userLookup,
            ICacheInHttpContext cacheInHttpContext,
            IUserImpersonationService userImpersonationService
        )
        {
            _contextAccessor = contextAccessor;
            _userLookup = userLookup;
            _cacheInHttpContext = cacheInHttpContext;
            _userImpersonationService = userImpersonationService;
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

                // if they're not in a web request with a claims principal, we're done
                var principal = _contextAccessor.HttpContext?.User;
                if (principal == null)
                    return null;

                // see if they're impersonating someone
                var impersonatedUser = await _userImpersonationService.GetCurrentlyImpersonatedUser(principal);

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
