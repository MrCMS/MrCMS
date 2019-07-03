using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Helpers;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class LoginController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly ILoginService _loginService;
        private readonly ILogUserIn _logUserIn;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly ISetVerifiedUserData _setVerifiedUserData;
        private readonly IEventContext _eventContext;
        private readonly ILogger<LoginController> _logger;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IUniquePageService _uniquePageService;
        private readonly IUserLookup _userLookup;

        public LoginController(IResetPasswordService resetPasswordService, IUniquePageService uniquePageService,
            ILoginService loginService, IStringResourceProvider stringResourceProvider, IUserLookup userLookup,
            ILogUserIn logUserIn, ISetVerifiedUserData setVerifiedUserData,
            IEventContext eventContext, ILogger<LoginController> logger)
        {
            _resetPasswordService = resetPasswordService;
            _uniquePageService = uniquePageService;
            _loginService = loginService;
            _stringResourceProvider = stringResourceProvider;
            _userLookup = userLookup;
            _logUserIn = logUserIn;
            _setVerifiedUserData = setVerifiedUserData;
            _eventContext = eventContext;
            _logger = logger;
        }

        [HttpGet]
        [CanonicalLinks]
        public ViewResult Show(LoginPage page, LoginModel model)
        {
            ModelState.Clear();
            ViewData["login-model"] = TempData.Get<LoginModel>() ?? model;
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<RedirectResult> Post(
            LoginModel loginModel)
        {
            if (loginModel != null && ModelState.IsValid)
            {
                var result = _loginService.AuthenticateUser(loginModel);
                switch (result.Status)
                {
                    case LoginStatus.Success:
                        await _logUserIn.Login(result.User, loginModel.RememberMe);
                        return Redirect(result.ReturnUrl);
                    case LoginStatus.TwoFactorRequired:
                        // if the page doesn't exist, do the standard login
                        if (_uniquePageService.GetUniquePage<TwoFactorCodePage>() == null)
                        {
                            await _logUserIn.Login(result.User, loginModel.RememberMe);
                            return Redirect(result.ReturnUrl);
                        }

                        _setVerifiedUserData.SetUserData(result.User);
                        return _uniquePageService.RedirectTo<TwoFactorCodePage>(new { result.ReturnUrl });
                    case LoginStatus.Failure:
                        _eventContext.Publish<IOnFailedLogin, UserFailedLoginEventArgs>(
                            new UserFailedLoginEventArgs(result.User, loginModel.Email));
                        break;
                    case LoginStatus.LockedOut:
                        _eventContext.Publish<IOnLockedOutUserAuthed, UserLockedOutEventArgs>(
                            new UserLockedOutEventArgs(result.User));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                loginModel.Message = result.Message;
            }

            TempData.Set(loginModel);

            return _uniquePageService.RedirectTo<LoginPage>();
        }

        [HttpGet]
        public ViewResult ForgottenPassword(ForgottenPasswordPage page)
        {
            ViewData["message"] = TempData["message"];
            return View(page);
        }

        [HttpPost]
        public ActionResult ForgottenPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["message"] = _stringResourceProvider.GetValue("Login Email Not Recognized",
                    "Email not recognized.");
                return _uniquePageService.RedirectTo<ForgottenPasswordPage>();
            }

            var user = _userLookup.GetUserByEmail(email);

            if (user != null)
            {
                _resetPasswordService.SetResetPassword(user);
                TempData["message"] =
                    _stringResourceProvider.GetValue("Login Password Reset",
                        "We have sent password reset details to you. Please check your spam folder if this is not received shortly.");
            }
            else
            {
                TempData["message"] = _stringResourceProvider.GetValue("Login Email Not Recognized",
                    "Email not recognized.");
            }

            return _uniquePageService.RedirectTo<ForgottenPasswordPage>();
        }


        [HttpGet]
        public ActionResult PasswordReset(ResetPasswordPage page, Guid? id)
        {
            if (id == null)
            {
                return Redirect("~/");
            }

            var user = _userLookup.GetUserByResetGuid(id.Value);

            if (user == null)
            {
                return Redirect("~");
            }

            ViewData["ResetPasswordViewModel"] = new ResetPasswordViewModel(id.Value, user);

            return View(page);
        }

        [HttpPost]
        public RedirectResult PasswordReset(ResetPasswordViewModel model)
        {
            try
            {
                _resetPasswordService.ResetPassword(model);
                return _uniquePageService.RedirectTo<LoginPage>();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error logging in");
                return _uniquePageService.RedirectTo<LoginPage>();
            }
        }
    }
}