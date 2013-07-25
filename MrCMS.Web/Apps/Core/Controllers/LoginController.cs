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
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;
        private readonly IResetPasswordService _resetPasswordService;

        public LoginController(IUserService userService, IResetPasswordService resetPasswordService, IAuthorisationService authorisationService, IDocumentService documentService,
            ILoginService loginService)
        {
            _userService = userService;
            _resetPasswordService = resetPasswordService;
            _authorisationService = authorisationService;
            _documentService = documentService;
            _loginService = loginService;
        }

        [HttpGet]
        public ViewResult Show(LoginPage page)
        {
            ModelState.Clear();
            ViewData["login-model"] = TempData["login-model"] as LoginModel ?? new LoginModel();
            return View(page);
        }

        [HttpPost]
        public RedirectResult Post(LoginModel loginModel)
        {
            if (loginModel != null && ModelState.IsValid)
            {
                var result = _loginService.AuthenticateUser(loginModel);
                if (result.Success)
                    return Redirect(result.RedirectUrl);
                loginModel.Message = result.Message;
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
            if (string.IsNullOrEmpty(email))
            {
                TempData["message"] = "Email not recognized.";
                return Redirect("~/" + _documentService.GetUniquePage<ForgottenPasswordPage>().LiveUrlSegment);
            }

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