using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class ExternalLoginController : MrCMSAppUIController<CoreApp>
    {
        private const string XsrfKey = "XsrfId";
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IExternalLoginService _externalLoginService;
        private readonly IUniquePageService _uniquePageService;

        public ExternalLoginController(IAuthenticationManager authenticationManager,
                                       IExternalLoginService externalLoginService, IUniquePageService uniquePageService)
        {
            _authenticationManager = authenticationManager;
            _externalLoginService = externalLoginService;
            _uniquePageService = uniquePageService;
        }

        public PartialViewResult Providers(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            IEnumerable<AuthenticationDescription> externalAuthenticationTypes = _authenticationManager.GetExternalAuthenticationTypes();
            return PartialView(externalAuthenticationTypes);
        }

        [HttpPost]
        public ActionResult Login(string provider, string returnUrl)
        {
            Session.RemoveAll();
            // Request a redirect to the external login provider
            var redirectUri = Url.Action("Callback", "ExternalLogin",
                new { ReturnUrl = returnUrl ?? CurrentRequestData.HomePage.AbsoluteUrl });
            return new ChallengeResult(provider,
                                       redirectUri);
        }

        public async Task<ActionResult> Callback(string returnUrl)
        {
            ExternalLoginInfo externalLoginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            AuthenticateResult authenticateResult =
                await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            string email = _externalLoginService.GetEmail(authenticateResult);
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["login-model"] = new LoginModel
                                              {
                                                  Message =
                                                      "There was an error retrieving your email from the 3rd party provider"
                                              };
                return _uniquePageService.RedirectTo<LoginPage>();
            }
            if (await _externalLoginService.IsLoginAsync(externalLoginInfo))
            {
                await _externalLoginService.LoginAsync(externalLoginInfo, authenticateResult);
                return await _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }
            if (await _externalLoginService.UserExistsAsync(email))
            {
                await _externalLoginService.AssociateLoginToUserAsync(email, externalLoginInfo);
                await _externalLoginService.LoginAsync(externalLoginInfo, authenticateResult);
                return await _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }
            if (!_externalLoginService.RequiresAdditionalFieldsForRegistration())
            {
                await _externalLoginService.CreateUserAsync(email, externalLoginInfo);
                await _externalLoginService.LoginAsync(externalLoginInfo, authenticateResult);
                return await _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }

            return _uniquePageService.RedirectTo<LoginPage>();

        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri, string userId = null)
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
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}