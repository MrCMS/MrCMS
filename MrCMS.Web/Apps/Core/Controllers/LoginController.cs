using System;
using System.Web.Mvc;
using Elmah;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Website.Controllers;
using ResetPasswordViewModel = MrCMS.Web.Apps.Core.Models.ResetPasswordViewModel;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class LoginController : MrCMSAppUIController<CoreApp>
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IDocumentService _documentService;
        private readonly IUserService _userService;
        private readonly IResetPasswordService _resetPasswordService;

        public LoginController(IUserService userService, IResetPasswordService resetPasswordService, IAuthorisationService authorisationService, IDocumentService documentService)
        {
            _userService = userService;
            _resetPasswordService = resetPasswordService;
            _authorisationService = authorisationService;
            _documentService = documentService;
        }

        [HttpGet]
        public ViewResult Show(LoginPage page)
        {
            ViewData["login-model"] = TempData["login-model"] as LoginModel ?? new LoginModel();
            return View(page);
        }

        [HttpPost]
        public ActionResult Post(LoginModel loginModel)
        {
            if (loginModel != null && ModelState.IsValid)
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
                        return Redirect(loginModel.ReturnUrl ?? "~/");
                    }
                    loginModel.Message = "Incorrect email or password.";
                }
            }
            TempData["login-model"] = loginModel;

            return Redirect("~/" + _documentService.GetUniquePage<LoginPage>().LiveUrlSegment);
        }


        public RedirectResult Logout()
        {
            _authorisationService.Logout();
            return Redirect("~");
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
            var user = _userService.GetUserByEmail(email);

            if (user != null)
            {
                _resetPasswordService.SetResetPassword(user);
                TempData["message"] =
                "We have sent password reset details to you. Please check your spam folder if this is not received shortly.";
            }
            else
            {
                TempData["message"] = "Email not recognized.";
            }


            return Redirect("~/" + _documentService.GetUniquePage<ForgottenPasswordPage>().LiveUrlSegment);
        }


        

        [HttpGet]
        public ActionResult PasswordReset(ResetPasswordPage page, Guid id)
        {
            var user = _userService.GetUserByResetGuid(id);

            if (user == null)
                return Redirect("~");

            ViewData["ResetPasswordViewModel"] = new ResetPasswordViewModel(id, user);

            return View(page);
        }

        [HttpPost]
        public RedirectResult PasswordReset(ResetPasswordViewModel model)
        {
            try
            {
                _resetPasswordService.ResetPassword(model);
                return Redirect("~/" + _documentService.GetUniquePage<LoginPage>().LiveUrlSegment);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return Redirect("~/" + _documentService.GetUniquePage<LoginPage>().LiveUrlSegment);
            }
        }
    }
}