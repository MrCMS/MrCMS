using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;
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
        private readonly IStringResourceProvider _stringResourceProvider;

        public ExternalLoginController(IAuthenticationManager authenticationManager,
                                       IExternalLoginService externalLoginService, IUniquePageService uniquePageService, IStringResourceProvider stringResourceProvider)
        {
            _authenticationManager = authenticationManager;
            _externalLoginService = externalLoginService;
            _uniquePageService = uniquePageService;
            _stringResourceProvider = stringResourceProvider;
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
            var previousSession = CurrentRequestData.UserGuid;
            ExternalLoginInfo loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();
            // added the following lines
            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                var identity = _authenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
                var accessToken = identity.FindFirstValue("FacebookAccessToken");
                var fb = new FacebookClient(accessToken);
                dynamic myInfo = fb.Get("/me?fields=email"); // specify the email field
                loginInfo.Email = myInfo.email;
            }


            AuthenticateResult authenticateResult =
                await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);
            string email = loginInfo.Email ?? _externalLoginService.GetEmail(authenticateResult);
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["login-model"] = new LoginModel
                {
                    Message =
                        _stringResourceProvider.GetValue("3rd Party Auth Email Error",
                            "There was an error retrieving your email from the 3rd party provider")
                };
                return _uniquePageService.RedirectTo<LoginPage>();
            }
            if (await _externalLoginService.IsLoginAsync(loginInfo))
            {
                await _externalLoginService.LoginAsync(loginInfo, authenticateResult);
                EventContext.Instance.Publish<IOnUserLoggedIn, UserLoggedInEventArgs>(
                    new UserLoggedInEventArgs(CurrentRequestData.CurrentUser, previousSession));
                return await _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }
            if (await _externalLoginService.UserExistsAsync(email))
            {
                await _externalLoginService.AssociateLoginToUserAsync(email, loginInfo);
                await _externalLoginService.LoginAsync(loginInfo, authenticateResult);
                EventContext.Instance.Publish<IOnUserLoggedIn, UserLoggedInEventArgs>(
                    new UserLoggedInEventArgs(CurrentRequestData.CurrentUser, previousSession));
                return await _externalLoginService.RedirectAfterLogin(email, returnUrl);
            }
            if (!_externalLoginService.RequiresAdditionalFieldsForRegistration())
            {
                await _externalLoginService.CreateUserAsync(email, loginInfo);
                await _externalLoginService.LoginAsync(loginInfo, authenticateResult);
                EventContext.Instance.Publish<IOnUserRegistered, OnUserRegisteredEventArgs>(
                    new OnUserRegisteredEventArgs(CurrentRequestData.CurrentUser, previousSession));
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