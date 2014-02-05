using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class AuthorisationService : IAuthorisationService
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly UserManager<User> _userManager;

        public AuthorisationService(IAuthenticationManager authenticationManager, UserManager<User> userManager)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        public void SetAuthCookie(User user, bool rememberMe)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }

        public void Logout()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}