using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MrCMS.Entities.People;
using MrCMS.Services.Auth;

namespace MrCMS.Services
{
    public class AuthorisationService : IAuthorisationService
    {
        private readonly ISignInManager _signInManager;

        public AuthorisationService(ISignInManager signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task SetAuthCookie(User user, bool rememberMe)
        {
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, new AuthenticationProperties {IsPersistent = rememberMe});
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}