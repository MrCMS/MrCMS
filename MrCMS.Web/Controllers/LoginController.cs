using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class LoginController : MrCMSUIController
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IUserService _userService;
        private readonly IResetPasswordService _resetPasswordService;

        public LoginController(IUserService userService, IResetPasswordService resetPasswordService, IAuthorisationService authorisationService)
        {
            _userService = userService;
            _resetPasswordService = resetPasswordService;
            _authorisationService = authorisationService;
        }

        [HttpGet]
        [ActionName("Login")]
        public ViewResult Get(LoginModel loginModel)
        {
            return View(loginModel);
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult Post(LoginModel loginModel)
        {
            if (loginModel != null)
            {
                User user = _userService.GetUserByEmail(loginModel.Email);
                if (user != null && user.IsActive)
                {
                    if (_authorisationService.ValidateUser(user, loginModel.Password))
                    {
                        _authorisationService.SetAuthCookie(loginModel.Email, loginModel.RememberMe);
                        if (user.IsAdmin)
                        {
                            return Redirect(loginModel.ReturnUrl ?? "~/admin");
                        }
                        else
                        {
                            return Redirect(loginModel.ReturnUrl ?? "~/");
                        }
                    }
                }
            }
            loginModel = loginModel ?? new LoginModel();
            return View(new LoginModel()
                {
                    Email = loginModel.Email,
                    RememberMe = loginModel.RememberMe,
                    ReturnUrl = loginModel.ReturnUrl,
                });
        }


        public ActionResult Logout()
        {
            _authorisationService.Logout();
            return Redirect("~");
        }

        [HttpGet]
        public ActionResult ForgottenPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgottenPassword(string email)
        {
            var user = _userService.GetUserByEmail(email);

            if (user != null)
            {
                _resetPasswordService.SetResetPassword(user);
            }

            return RedirectToAction("ForgottenPasswordSent");
        }

        public ActionResult ForgottenPasswordSent()
        {
            return View();
        }

        #region Nested type: LoginModel

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public bool RememberMe { get; set; }
            public string ReturnUrl { get; set; }
        }

        #endregion

        [HttpGet]
        public ActionResult PasswordReset(Guid id)
        {
            var user = _userService.GetUserByResetGuid(id);

            if (user == null)
                return Redirect("~");

            var model = new ResetPasswordViewModel(id, user);

            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordReset(ResetPasswordViewModel model)
        {
            try
            {
                _resetPasswordService.ResetPassword(model);
                return RedirectToAction("ResetComplete");
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
                return RedirectToAction("PasswordReset", model.Id);
            }
        }

        public ActionResult ResetComplete()
        {
            return View();
        }

    }
}