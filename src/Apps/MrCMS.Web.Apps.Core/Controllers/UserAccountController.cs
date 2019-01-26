using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class UserAccountController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IUniquePageService _uniquePageService;
        private readonly IUserManagementService _userManagementService;

        public UserAccountController(IUserManagementService userManagementService,
            IPasswordManagementService passwordManagementService, IAuthorisationService authorisationService,
            IStringResourceProvider stringResourceProvider,
            IGetCurrentUser getCurrentUser, IUniquePageService uniquePageService)
        {
            _userManagementService = userManagementService;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
            _stringResourceProvider = stringResourceProvider;
            _getCurrentUser = getCurrentUser;
            _uniquePageService = uniquePageService;
        }

        [CanonicalLinks]
        public ActionResult Show(UserAccountPage page)
        {
            ViewData["message"] = TempData["message"];

            if (_getCurrentUser.Get() != null) return View(page);

            return _uniquePageService.RedirectTo<LoginPage>();
        }

        [HttpGet]
        public ActionResult UserAccountDetails(UserAccountModel model)
        {
            var user = _getCurrentUser.Get();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
                ModelState.Clear();
                return View(model);
            }

            return _uniquePageService.RedirectTo<LoginPage>();
        }

        [HttpPost]
        [ActionName("UserAccountDetails")]
        public async Task<RedirectResult> UserAccountDetails_POST(UserAccountModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = _getCurrentUser.Get();
                if (user != null && user.IsActive)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    _userManagementService.SaveUser(user);
                    await _authorisationService.SetAuthCookie(user, false);

                    return _uniquePageService.RedirectTo<UserAccountPage>();
                }
            }

            return _uniquePageService.RedirectTo<UserAccountPage>();
        }

        public JsonResult IsUniqueEmail(string email)
        {
            return _userManagementService.IsUniqueEmail(email, _getCurrentUser.Get()?.Id)
                ? Json(true)
                : Json(_stringResourceProvider.GetValue("Register Email Already Registered",
                    "Email already registered."));
        }

        [HttpGet]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            ModelState.Clear();
            return View(model);
        }

        [HttpPost]
        [ActionName("ChangePassword")]
        public RedirectResult ChangePassword_POST(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _getCurrentUser.Get();
                _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);
                model.Message = _stringResourceProvider.GetValue("Login Password Updated", "Password updated.");
            }
            else
            {
                model.Message = _stringResourceProvider.GetValue("Login Invalid",
                    "Please ensure both fields are filled out and valid");
            }

            TempData["message"] = model.Message;
            return _uniquePageService.RedirectTo<UserAccountPage>();
        }
    }
}