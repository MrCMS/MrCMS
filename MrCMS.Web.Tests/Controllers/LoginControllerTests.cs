using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Controllers;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Web.Controllers;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Controllers
{
    public class LoginControllerTests : MrCMSTest
    {
        private readonly IUserService _userService;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IDocumentService _documentService;
        private readonly ILoginService _loginService;
        private readonly LoginController _loginController;

        public LoginControllerTests()
        {
            _userService = A.Fake<IUserService>();
            _resetPasswordService = A.Fake<IResetPasswordService>();
            _authorisationService = A.Fake<IAuthorisationService>();
            _documentService = A.Fake<IDocumentService>();
            _loginService = A.Fake<ILoginService>();
            _loginController = new LoginController(_userService, _resetPasswordService, _authorisationService, _documentService, _loginService);
            // initial setup as this is reused
            A.CallTo(() => _documentService.GetUniquePage<LoginPage>())
             .Returns(new LoginPage { UrlSegment = "login-page" });
        }

        [Fact]
        public void LoginController_Show_ShouldReturnLoginPageAsModel()
        {
            var result = _loginController.Show(new LoginPage { Layout = new Layout() });

            result.Model.Should().BeOfType<LoginPage>();
        }

        //[Fact]
        //public void LoginController_Get_ShouldReturnPassedModel()
        //{
        //    var loginModel = new LoginController.LoginModel();

        //    var result = _loginController.Get(loginModel);

        //    result.Model.Should().Be(loginModel);
        //}

        [Fact]
        public void LoginController_Post_IfModelIsNullReturnsRedirectToLoginPage()
        {
            var actionResult = _loginController.Post(null);

            actionResult.Should().NotBeNull();
            actionResult.Url.Should().Be("~/login-page");
        }

        [Fact]
        public void LoginController_Post_IfModelIsNotNullButModelStateIsInvalidReturnRedirectToLoginPage()
        {
            _loginController.ModelState.AddModelError("test", "error");
            var actionResult = _loginController.Post(new LoginModel());

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
             .Returns(new LoginResult { Success = true, RedirectUrl = "redirect-url" });

            var redirectResult = _loginController.Post(loginModel);

            redirectResult.Url.Should().Be("redirect-url");
        }

        [Fact]
        public void LoginController_Post_IfAuthenticateUserReturnsFailureRedirectToLoginPage()
        {
            var loginModel = new LoginModel();
            A.CallTo(() => _loginService.AuthenticateUser(loginModel))
             .Returns(new LoginResult { Success = false });

            var redirectResult = _loginController.Post(loginModel);

            redirectResult.Url.Should().Be("~/login-page");
        }

        [Fact]
        public void LoginController_Post_IfAuthenticateUserReturnsFailureShouldSetMessageToModel()
        {
            var loginModel = new LoginModel();
            A.CallTo(() => _loginService.AuthenticateUser(loginModel))
             .Returns(new LoginResult { Success = false, Message = "failure message" });

            _loginController.Post(loginModel);

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