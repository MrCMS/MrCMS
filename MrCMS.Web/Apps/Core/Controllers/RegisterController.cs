using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class RegistrationController : MrCMSAppUIController<CoreApp>
    {
        private readonly IUserService _userService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IAuthorisationService _authorisationService;

        public RegistrationController(IUserService userService, IPasswordManagementService passwordManagementService, IAuthorisationService authorisationService)
        {
            _userService = userService;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
        }

        public ActionResult Show(RegisterPage page)
        {
            if (CurrentRequestData.CurrentUser != null)
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
        public ActionResult RegistrationDetails_POST(RegisterModel model)
        {
            if (CurrentRequestData.CurrentUser != null)
            {
                TempData["already-logged-in"] = "You are already logged in. Please logout to create a new account.";
                return Redirect(UniquePageHelper.GetUrl<RegisterPage>());
            }

            if (model != null && ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsActive = true
                };
                _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);
                _userService.AddUser(user);
                _authorisationService.SetAuthCookie(model.Email, false);
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                    return Redirect("~/" + model.ReturnUrl);
                return Redirect("~/");
            }
            return Redirect(UniquePageHelper.GetUrl<RegisterPage>());
        }

        public JsonResult CheckEmailIsNotRegistered(string email)
        {
            return Json(_userService.GetUserByEmail(email) == null, JsonRequestBehavior.AllowGet);
        }
    }
}