using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Helpers;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class ExternalLoginController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IExternalLoginService _externalLoginService;
        private readonly IUniquePageService _uniquePageService;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IGetVerifiedUserResult _getVerifiedUserResult;
        private readonly ILogUserIn _logUserIn;
        private readonly ISetVerifiedUserData _setVerifiedUserData;
        private readonly ISignInManager _signInManager;
        private readonly IEventContext _eventContext;

        public ExternalLoginController(
                                       IExternalLoginService externalLoginService, IUniquePageService uniquePageService, IStringResourceProvider stringResourceProvider,
                                       IGetVerifiedUserResult getVerifiedUserResult,
                                       ILogUserIn logUserIn, ISetVerifiedUserData setVerifiedUserData, ISignInManager signInManager,
                                       IEventContext eventContext)
        {
            _externalLoginService = externalLoginService;
            _uniquePageService = uniquePageService;
            _stringResourceProvider = stringResourceProvider;
            _getVerifiedUserResult = getVerifiedUserResult;
            _logUserIn = logUserIn;
            _setVerifiedUserData = setVerifiedUserData;
            _signInManager = signInManager;
            _eventContext = eventContext;
        }

        public async Task<PartialViewResult> Providers(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            IEnumerable<AuthenticationScheme> externalAuthenticationTypes =
                await _signInManager.GetExternalAuthenticationSchemesAsync();
            return PartialView(externalAuthenticationTypes);
        }

        [HttpPost]
        public ActionResult Login(string provider, string returnUrl)
        {
            HttpContext.Session.Clear();
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, Url.Action("Callback", "ExternalLogin", new { ReturnUrl = returnUrl }));
            return Challenge(properties, provider);
        }

        public async Task<ActionResult> Callback(string returnUrl, string remoteError = null)
        {
            var result = await _externalLoginService.HandleExternalLoginCallback(returnUrl, remoteError);
            if (!result.Success)
            {
                TempData["error-message"] = result.Error;
                return RedirectToAction(nameof(Login));
            }

            var signInResult = result.Result;
            if (signInResult.Succeeded)
            {
                //_logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return this.RedirectToLocal(returnUrl);
            }
            //if (signInResult.RequiresTwoFactor)
            //{
            //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
            //}
            //if (signInResult.IsLockedOut)
            //{
            //    return View("Lockout");
            //}
            ////ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            ////if (loginInfo == null)
            ////    return ThirdPartyError();

            ////AuthenticateResult authenticateResult = 
            ////    await _authenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ExternalCookie);

            ////loginInfo.Email = loginInfo.Email ?? _externalLoginService.GetEmail(authenticateResult);

            ////if (string.IsNullOrWhiteSpace(loginInfo.Email))
            ////    return ThirdPartyError();

            var user = await _externalLoginService.GetUserToLogin(result.LoginInfo);

            var loginResult = _getVerifiedUserResult.GetResult(user, returnUrl);
            switch (loginResult.Status)
            {
                case LoginStatus.Success:
                    await _logUserIn.Login(user, false);
                    await _externalLoginService.UpdateClaimsAsync(user, result.LoginInfo.Principal.Claims);
                    return Redirect(loginResult.ReturnUrl);
                case LoginStatus.TwoFactorRequired:
                    // if the page doesn't exist, do the standard login
                    if (_uniquePageService.GetUniquePage<TwoFactorCodePage>() == null)
                    {
                        await _logUserIn.Login(user, false);
                        await _externalLoginService.UpdateClaimsAsync(user, result.LoginInfo.Principal.Claims);
                        return Redirect(loginResult.ReturnUrl);
                    }
                    _setVerifiedUserData.SetUserData(loginResult.User);
                    return _uniquePageService.RedirectTo<TwoFactorCodePage>(new { loginResult.ReturnUrl });
                case LoginStatus.LockedOut:
                    _eventContext.Publish<IOnLockedOutUserAuthed, UserLockedOutEventArgs>(
                        new UserLockedOutEventArgs(loginResult.User));
                    break;
            }

            TempData.Set(new LoginModel {Message = loginResult.Message});
            return _uniquePageService.RedirectTo<LoginPage>();
        }

        private ActionResult ThirdPartyError()
        {
            TempData.Set(new LoginModel
            {
                Message =
                    _stringResourceProvider.GetValue("3rd Party Auth Email Error",
                        "There was an error retrieving your email from the 3rd party provider")
            });
            return _uniquePageService.RedirectTo<LoginPage>();
        }
    }
}