using System;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;

namespace MrCMS.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthorisationService _authorisationService;
        private readonly IUserService _userService;

        public LoginController(IUserService userService, AuthorisationService authorisationService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
        }

        [HttpGet]
        [ActionName("Login")]
        public ActionResult Get(LoginModel loginModel)
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
                        return Redirect(loginModel.RedirectUrl ?? "~/admin");
                    }
                }
            }
            loginModel = loginModel ?? new LoginModel();
            return View(new LoginModel()
                {
                    Email = loginModel.Email,
                    RememberMe = loginModel.RememberMe,
                    RedirectUrl = loginModel.RedirectUrl,
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
                _userService.SetResetPassword(user);
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
            public string RedirectUrl { get; set; }
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
                _userService.ResetPassword(model);
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