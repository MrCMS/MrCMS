using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Controllers;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Controllers
{
    public class LoginControllerTests : MrCMSTest
    {
        private readonly IUserService _userService;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly IAuthorisationService _authorisationService;
        private readonly ILoginService _loginService;
        private readonly LoginController _loginController;
        private readonly IUniquePageService _uniquePageService;

        public LoginControllerTests()
        {
            _userService = A.Fake<IUserService>();
            _resetPasswordService = A.Fake<IResetPasswordService>();
            _authorisationService = A.Fake<IAuthorisationService>();
            _loginService = A.Fake<ILoginService>();
            _uniquePageService = A.Fake<IUniquePageService>();
            _loginController = new LoginController(_userService, _resetPasswordService, _authorisationService, _uniquePageService, _loginService);

            // initial setup as this is reused
            A.CallTo(() => _uniquePageService.RedirectTo<LoginPage>(null)).Returns(new RedirectResult("~/login-page"));
            A.CallTo(() => _uniquePageService.RedirectTo<ForgottenPasswordPage>(null))
             .Returns(new RedirectResult("~/forgotten-password"));
        }

        [Fact]
        public void LoginController_Show_ShouldReturnLoginPageAsModel()
        {
            var result = _loginController.Show(new LoginPage(), new LoginModel());

            result.Model.Should().BeOfType<LoginPage>();
        }

        [Fact]
        public void LoginController_Show_ShouldSetViewDataAsModel()
        {
            var loginModel = new LoginModel();

            var result = _loginController.Show(null, loginModel);

            result.ViewData["login-model"].Should().Be(loginModel);
        }

        [Fact]
        public void LoginController_Show_ShouldUseTempDataModelBeforePassedModel()
        {
            var model1 = new LoginModel();
            var model2 = new LoginModel();
            _loginController.TempData["login-model"] = model1;

            var result = _loginController.Show(null, model2);

            result.ViewData["login-model"].Should().Be(model1);
        }

        [Fact]
        public void LoginController_Post_IfModelIsNullReturnsRedirectToLoginPage()
        {
            var actionResult = _loginController.Post(null).Result;

            actionResult.Should().NotBeNull();
            actionResult.Url.Should().Be("~/login-page");
        }

        [Fact]
        public void LoginController_Post_IfModelIsNotNullButModelStateIsInvalidReturnRedirectToLoginPage()
        {
            _loginController.ModelState.AddModelError("test", "error");
            var actionResult = _loginController.Post(new LoginModel()).Result;

            actionResult.Should().NotBeNull();
            actionResult.Url.Should().Be("~/login-page");
        }

        [Fact]
        public void LoginController_Post_IfModelIsNotNullButModelStateIsInvalidShouldNotCallAuthenticateUser()
        {
            _loginController.ModelState.AddModelError("test", "error");
            var loginModel = new LoginModel();
            _loginController.Post(loginModel);

            A.CallTo(() => _loginService.AuthenticateUser(loginModel)).MustNotHaveHappened();
        }

        [Fact]
        public void LoginController_Post_IfAuthenticateUserReturnsSuccessShouldRedirectToReturnedUrl()
        {
            var loginModel = new LoginModel();
            A.CallTo(() => _loginService.AuthenticateUser(loginModel))
             .Returns(Task.Run(() => new LoginResult { Success = true, RedirectUrl = "redirect-url" }));
            var redirectResult = _loginController.Post(loginModel).Result;

            redirectResult.Url.Should().Be("redirect-url");
        }

        [Fact]
        public void LoginController_Post_IfAuthenticateUserReturnsFailureRedirectToLoginPage()
        {
            var loginModel = new LoginModel();
            A.CallTo(() => _loginService.AuthenticateUser(loginModel))
             .Returns(Task.Run(() => new LoginResult {Success = false}));

            var redirectResult = _loginController.Post(loginModel).Result;

            redirectResult.Url.Should().Be("~/login-page");
        }

        [Fact]
        public void LoginController_Post_IfAuthenticateUserReturnsFailureShouldSetMessageToModel()
        {
            var loginModel = new LoginModel();
            A.CallTo(() => _loginService.AuthenticateUser(loginModel))
             .Returns(Task.Run(() => new LoginResult {Success = false, Message = "failure message"}));

            RedirectResult redirectResult = _loginController.Post(loginModel).Result;

            loginModel.Message.Should().Be("failure message");
        }

        [Fact]
        public void LoginController_Logout_CallsAuthorisationServiceLogout()
        {
            _loginController.Logout();

            A.CallTo(() => _authorisationService.Logout()).MustHaveHappened();
        }

        [Fact]
        public void LoginController_Logout_RedirectsToRoute()
        {
            var result = _loginController.Logout();

            result.Url.Should().Be("~");
        }

        [Fact]
        public void LoginController_ForgottenPasswordGET_ShouldReturnAView()
        {
            _loginController.ForgottenPassword("").Should().NotBeNull();
        }

        [Fact]
        public void LoginController_ForgottenPasswordPOST_ShouldCallGetUserByEmailWithPassedEmail()
        {
            var forgottenPassword = _loginController.ForgottenPassword("test@example.com");

            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).MustHaveHappened();
        }
    }
}