using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Services.Resources;
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
        private readonly IGetVerifiedUserResult _getVerifiedUserResult;
        private readonly ILogUserIn _logUserIn;
        private readonly ISetVerifiedUserData _setVerifiedUserData;

        public ExternalLoginController(IAuthenticationManager authenticationManager,
                                       IExternalLoginService externalLoginService, IUniquePageService uniquePageService, IStringResourceProvider stringResourceProvider,
                                       IGetVerifiedUserResult getVerifiedUserResult,
                                       ILogUserIn logUserIn, ISetVerifiedUserData setVerifiedUserData)
        {
            _authenticationManager = authenticationManager;
            _externalLoginService = externalLoginService;
            _uniquePageService = uniquePageService;
            _stringResourceProvider = stringResourceProvider;
            _getVerifiedUserResult = getVerifiedUserResult;
            _logUserIn = logUserIn;
            _setVerifiedUserData = setVerifiedUserData;
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
            ExternalLoginInfo loginInfo = await _authenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo?.Login == null)
                return ThirdPartyError();

            AuthenticateResult authenticateResult =
                await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            loginInfo.Email = loginInfo.Email ?? _externalLoginService.GetEmail(authenticateResult);

            if (string.IsNullOrWhiteSpace(loginInfo.Email))
                return ThirdPartyError();

            var user = await _externalLoginService.GetUserToLogin(loginInfo);

            var result = _getVerifiedUserResult.GetResult(user, returnUrl);
            switch (result.Status)
            {
                case LoginStatus.Success:
                    await _logUserIn.Login(user, false);
                    await _externalLoginService.UpdateClaimsAsync(user, authenticateResult.Identity.Claims);
                    return Redirect(result.ReturnUrl);
                case LoginStatus.TwoFactorRequired:
                    // if the page doesn't exist, do the standard login
                    if (_uniquePageService.GetUniquePage<TwoFactorCodePage>() == null)
                    {
                        await _logUserIn.Login(user, false);
                        await _externalLoginService.UpdateClaimsAsync(user, authenticateResult.Identity.Claims);
                        return Redirect(result.ReturnUrl);
                    }
                    _setVerifiedUserData.SetUserData(result.User);
                    return _uniquePageService.RedirectTo<TwoFactorCodePage>(new { result.ReturnUrl });
                case LoginStatus.LockedOut:
                    EventContext.Instance.Publish<IOnLockedOutUserAuthed, UserLockedOutEventArgs>(
                        new UserLockedOutEventArgs(result.User));
                    break;
            }
            TempData["login-model"] = new LoginModel { Message = result.Message };
            return _uniquePageService.RedirectTo<LoginPage>();
        }

        private ActionResult ThirdPartyError()
        {
            TempData["login-model"] = new LoginModel
            {
                Message =
                    _stringResourceProvider.GetValue("3rd Party Auth Email Error",
                        "There was an error retrieving your email from the 3rd party provider")
            };
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