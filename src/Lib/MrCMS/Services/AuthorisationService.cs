using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MrCMS.Entities.People;
using MrCMS.Services.Auth;

namespace MrCMS.Services
{
    public class AuthorisationService : IAuthorisationService
    {
        private readonly ISignInManager _signInManager;
        private readonly IEventContext _eventContext;

        public AuthorisationService(ISignInManager signInManager, IEventContext eventContext)
        {
            _signInManager = signInManager;
            _eventContext = eventContext;
        }

        public async Task SetAuthCookie(User user, bool rememberMe)
        {
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, new AuthenticationProperties {IsPersistent = rememberMe});
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            _eventContext.Publish<IOnLoggedOut, LoggedOutEventArgs>(new LoggedOutEventArgs());
        }
    }
}