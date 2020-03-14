using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class RegistrationController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IRegistrationService _registrationService;
        private readonly IStringResourceProvider _stringResourceProvider;
        private readonly IUniquePageService _uniquePageService;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IUserRoleManager _userRoleManager;

        public RegistrationController(IRegistrationService registrationService,
            IStringResourceProvider stringResourceProvider,
            IUniquePageService uniquePageService,
            IGetCurrentUser getCurrentUser,
            IUserRoleManager userRoleManager)
        {
            _registrationService = registrationService;
            _stringResourceProvider = stringResourceProvider;
            _uniquePageService = uniquePageService;
            _getCurrentUser = getCurrentUser;
            _userRoleManager = userRoleManager;
        }

        [CanonicalLinks]
        public async Task<ActionResult> Show(RegisterPage page)
        {
            var user = _getCurrentUser.Get();
            if (user != null && !await _userRoleManager.IsAdmin(user))
                return Redirect("~/");
            return View(page);
        }

        [HttpGet]
        public ViewResult RegistrationDetails(RegisterModel model, string returnUrl = null)
        {
            ViewBag.Message = TempData["already-logged-in"];
            ModelState.Clear();
            model.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ActionName("RegistrationDetails")]
        public async Task<RedirectResult> RegistrationDetails_POST(RegisterModel model)
        {
            var user = _getCurrentUser.Get();
            if (user != null)
            {
                TempData["already-logged-in"] = _stringResourceProvider.GetValue("Register Already Logged in",
                    "You are already logged in. Please logout to create a new account.");
                return await _uniquePageService.RedirectTo<RegisterPage>();
            }

            if (model != null && ModelState.IsValid && _registrationService.CheckEmailIsNotRegistered(model.Email))
            {
                await _registrationService.RegisterUser(model);

                return !string.IsNullOrEmpty(model.ReturnUrl)
                    ? Redirect("~/" + model.ReturnUrl)
                    : Redirect("~/");
            }

            model ??= new RegisterModel();

            model.Password = null;
            model.ConfirmPassword = null;

            TempData["register-model"] = model;

            return await _uniquePageService.RedirectTo<RegisterPage>();
        }

        public JsonResult CheckEmailIsNotRegistered(string email)
        {
            return Json(_registrationService.CheckEmailIsNotRegistered(email));
        }
    }
}