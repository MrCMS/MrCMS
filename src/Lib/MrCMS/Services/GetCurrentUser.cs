using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class GetCurrentUser : IGetCurrentUser
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserLookup _userLookup;
        private readonly IUserImpersonationService _userImpersonationService;

        public GetCurrentUser(IHttpContextAccessor contextAccessor,
            IUserLookup userLookup,
            IUserImpersonationService userImpersonationService
        )
        {
            _contextAccessor = contextAccessor;
            _userLookup = userLookup;
            _userImpersonationService = userImpersonationService;
        }

        public async Task<User> Get()
        {
            return await GetImpersonatedUser() ?? await GetLoggedInUser();
        }

        public async Task<User> GetLoggedInUser()
        {
            // if the current context is null, we're done
            if (_contextAccessor.HttpContext == null)
                return null;

            // check for the current CurrentUserFeature
            var currentUserFeature = _contextAccessor.HttpContext.Features.Get<CurrentUserFeature>();
            if (currentUserFeature != null)
                return currentUserFeature.User;

            // otherwise we'll get the current user for the claims principal
            var user = await _userLookup.GetCurrentUser(_contextAccessor.HttpContext.User);

            // and cache it in the CurrentUserFeature
            currentUserFeature = new CurrentUserFeature
            {
                User = user
            };
            _contextAccessor.HttpContext.Features.Set(currentUserFeature);

            return user;
        }

        public async Task<User> GetImpersonatedUser()
        {
            // if the current context is null, we're done
            if (_contextAccessor.HttpContext == null)
                return null;

            // otherwise we'll get the current user for the claims principal
            var loggedInUser = await GetLoggedInUser();

            // if this is null, we're done
            if (loggedInUser == null)
                return null;

            // check for the current CurrentImpersonatedUserFeature
            var currentImpersonatedUserFeature =
                _contextAccessor.HttpContext.Features.Get<CurrentImpersonatedUserFeature>();

            // if we have a current impersonated user, we're done
            if (currentImpersonatedUserFeature != null)
                return currentImpersonatedUserFeature.User;

            // otherwise we'll get the current impersonated user
            var impersonatedUser =
                await _userImpersonationService.GetCurrentlyImpersonatedUser(_contextAccessor.HttpContext.User);

            // and cache it in the CurrentImpersonatedUserFeature
            currentImpersonatedUserFeature = new CurrentImpersonatedUserFeature
            {
                User = impersonatedUser
            };
            _contextAccessor.HttpContext.Features.Set(currentImpersonatedUserFeature);

            return impersonatedUser;
        }

        private class CurrentUserFeature
        {
            [CanBeNull] public User User { get; set; }
        }

        private class CurrentImpersonatedUserFeature
        {
            [CanBeNull] public User User { get; set; }
        }
    }
}