using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using System.Linq;

namespace MrCMS.Web.Controllers
{
    public class ExternalLoginController : MrCMSUIController
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IExternalLoginService _externalLoginService;
        private readonly IUniquePageService _uniquePageService;

        public ExternalLoginController(UserManager<User> userManager, IAuthenticationManager authenticationManager, IExternalLoginService externalLoginService, IUniquePageService uniquePageService)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
            _externalLoginService = externalLoginService;
            _uniquePageService = uniquePageService;
        }


        [HttpPost]
        public ActionResult Login(string provider, string returnUrl)
        {
            ControllerContext.HttpContext.Session.RemoveAll();
            // Request a redirect to the external login provider
            return new ChallengeResult(provider,
                                       Url.Action("Callback", "ExternalLogin",
                                                  new {ReturnUrl = returnUrl ?? CurrentRequestData.HomePage.AbsoluteUrl}));
        }

        public async Task<ActionResult> Callback(string returnUrl)
        {
            AuthenticateResult result = await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            IEnumerable<Claim> claims = result.Identity.Claims ?? new List<Claim>();
            var emailClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var email = emailClaim != null ? emailClaim.Value : null;
            if (email == null)
            {
                return _uniquePageService.RedirectTo<LoginPage>();
            }

            var loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return _uniquePageService.RedirectTo<LoginPage>();
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login) ?? await _userManager.FindByNameAsync(email);
            if (user == null)
            {
                user = new User { Email = email, IsActive = true };
                IdentityResult identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    TempData["login-model"] = new LoginModel { Message = string.Join(", ", identityResult.Errors) };
                    return _uniquePageService.RedirectTo<LoginPage>();
                }
                foreach (var claim in claims)
                {
                    _userManager.AddClaim(user.OwinId, claim);
                }
            }
            await SignInAsync(user, isPersistent: false);
            return RedirectToLocal(returnUrl);
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
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

    public interface IExternalLoginService
    {
    }

    public class ExternalLoginService : IExternalLoginService
    {
    }
}