using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using System.Linq;

namespace MrCMS.Services
{
    public class AuthorisationService : IAuthorisationService
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IUserManager  _userManager;

        public AuthorisationService(IAuthenticationManager authenticationManager, IUserManager userManager)
        {
            _authenticationManager = authenticationManager;
            _userManager = userManager;
        }

        public async Task SetAuthCookie(User user, bool rememberMe)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = rememberMe }, identity);
        }

        public void Logout()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

    }
}