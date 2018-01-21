using System;
using System.Web.Mvc;
using MrCMS.Attributes;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TwoFactorCodePageController : MrCMSAppUIController<CoreApp>
    {
        private readonly ITwoFactorConfirmationService _confirmationService;
        private readonly IUniquePageService _uniquePageService;
        private readonly ILogUserIn _logUserIn;

        public TwoFactorCodePageController(ITwoFactorConfirmationService confirmationService, IUniquePageService uniquePageService, ILogUserIn logUserIn)
        {
            _confirmationService = confirmationService;
            _uniquePageService = uniquePageService;
            _logUserIn = logUserIn;
        }
        [CanonicalLinks]
        public ActionResult Show(TwoFactorCodePage page, TwoFactorAuthModel model)
        {
            ModelState.Clear();
            var status = _confirmationService.GetStatus();
            switch (status)
            {
                case TwoFactorStatus.Valid:
                    ViewData["2fa-model"] = model;
                    return View(page);
                case TwoFactorStatus.Expired:
                    TempData["login-model"] = new LoginModel { Message = "Two-factor token expired, please try again." };
                    return _uniquePageService.RedirectTo<LoginPage>();
                default:
                    return _uniquePageService.RedirectTo<LoginPage>();
            }
        }

        [HttpPost]
        public ActionResult Post(TwoFactorAuthModel model)
        {
            var result = _confirmationService.TryAndConfirmCode(model);
            if (result.Success)
            {
                _logUserIn.Login(result.User, false); //TODO: pass this over
                return Redirect(result.ReturnUrl);
            }
            return _uniquePageService.RedirectTo<TwoFactorCodePage>(new { result.ReturnUrl });
        }
    }

}