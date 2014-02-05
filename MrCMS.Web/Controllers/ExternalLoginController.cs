using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using MrCMS.Website.Controllers;
using System.Linq;

namespace MrCMS.Web.Controllers
{
    public class ExternalLoginController : MrCMSUIController
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authenticationManager;

        public ExternalLoginController(UserManager<User> userManager,IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
        }


        [HttpPost]
        public ActionResult Login(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("Callback", "ExternalLogin", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        public async Task<ActionResult> Callback(string returnUrl)
        {
            var result = await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            IEnumerable<Claim> claims = result.Identity.Claims;

            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                user= new User { Email = claims.First(claim => claim.Type == ClaimTypes.Email).Value };
                IdentityResult identityResult = await _userManager.CreateAsync(user);
                bool succeeded = identityResult.Succeeded;
            }
            await SignInAsync(user, isPersistent: false);
            foreach (var claim in claims)
            {
                _userManager.AddClaim(user.OwinId, claim);
            }
            return RedirectToLocal(returnUrl);
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return Redirect("~");
            }
        }
        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        private const string XsrfKey = "XsrfId";
    }

}