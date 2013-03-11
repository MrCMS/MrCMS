using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Controllers
{
    public class LoginControllerTests
    {
        private LoginController _loginController;
        private IUserService _userService;
        private IResetPasswordService _resetPasswordService;
        private IAuthorisationService _authorisationService;

        public LoginControllerTests()
        {
            _userService = A.Fake<IUserService>();
            _resetPasswordService = A.Fake<IResetPasswordService>();
            _authorisationService = A.Fake<IAuthorisationService>();
            _loginController = new LoginController(_userService, _resetPasswordService, _authorisationService);
        }

        [Fact]
        public void LoginController_Get_ShouldReturnAViewResult()
        {
            var result = _loginController.Get(null);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LoginController_Get_ShouldReturnPassedModel()
        {
            var loginModel = new LoginController.LoginModel();
            
            var result = _loginController.Get(loginModel);

            result.Model.Should().Be(loginModel);
        }

        [Fact]
        public void LoginController_Post_IfModelIsNullReturnsViewResult()
        {
            var actionResult = _loginController.Post(null);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LoginController_Post_IfModelIsNullReturnsModelOfLoginModel()
        {
            var actionResult = _loginController.Post(null);

            actionResult.As<ViewResult>().Model.Should().BeOfType<LoginController.LoginModel>();
        }

        [Fact]
        public void LoginController_Post_IfModelIsSetAndGetUserByEmailIsNullReturnsViewResult()
        {
            var loginModel = new LoginController.LoginModel { Email = "test@example.com" };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(null);

            var actionResult = _loginController.Post(loginModel);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LoginController_Post_IfModelIsSetAndGetUserByEmailIsNullPassesBackEmailRememberMeAndReturnUrl()
        {
            var loginModel = new LoginController.LoginModel
            {
                Email = "test@example.com",
                RememberMe = true,
                ReturnUrl = "test-url"
            };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(null);

            var actionResult = _loginController.Post(loginModel);

            var model = actionResult.As<ViewResult>().Model.As<LoginController.LoginModel>();
            model.Email.Should().Be("test@example.com");
            model.RememberMe.Should().BeTrue();
            model.ReturnUrl.Should().Be("test-url");
        }

        [Fact]
        public void LoginController_Post_IfModelIsSetAndGetUserByEmailIsInactiveUserlReturnsViewResult()
        {
            var loginModel = new LoginController.LoginModel { Email = "test@example.com" };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(new User{IsActive=false});

            var actionResult = _loginController.Post(loginModel);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LoginController_Post_IfModelIsSetAndGetUserByEmailIsInactiveUserPassesBackEmailRememberMeAndReturnUrl()
        {
            var loginModel = new LoginController.LoginModel
            {
                Email = "test@example.com",
                RememberMe = true,
                ReturnUrl = "test-url"
            };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(new User { IsActive = false });

            var actionResult = _loginController.Post(loginModel);

            var model = actionResult.As<ViewResult>().Model.As<LoginController.LoginModel>();
            model.Email.Should().Be("test@example.com");
            model.RememberMe.Should().BeTrue();
            model.ReturnUrl.Should().Be("test-url");
        }

        [Fact]
        public void LoginController_Post_IfActiveUserIsLoadedIfValidateUserIsFalseReturnViewResult()
        {
            var loginModel = new LoginController.LoginModel { Email = "test@example.com",Password = "test-pass"};
            var user = new User {IsActive = true};
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            A.CallTo(() => _authorisationService.ValidateUser(user, "test-pass")).Returns(false);

            var actionResult = _loginController.Post(loginModel);

            actionResult.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LoginController_Post_IfActiveUserIsLoadedIfValidateUserIsFalsePassesBackEmailRememberMeAndReturnUrl()
        {
            var loginModel = new LoginController.LoginModel
                                 {
                                     Email = "test@example.com",
                                     RememberMe = true,
                                     ReturnUrl = "test-url",
                                     Password = "test-pass"
                                 };
            var user = new User {IsActive = true};
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            A.CallTo(() => _authorisationService.ValidateUser(user, "test-pass")).Returns(false);

            var actionResult = _loginController.Post(loginModel);

            var model = actionResult.As<ViewResult>().Model.As<LoginController.LoginModel>();
            model.Email.Should().Be("test@example.com");
            model.RememberMe.Should().BeTrue();
            model.ReturnUrl.Should().Be("test-url");
        }

        [Fact]
        public void LoginController_Post_IfActiveUserIsLoadedIfValidateUserIsTrueCallSetAuthCookieWithEmailAndRememberMe()
        {
            var loginModel = new LoginController.LoginModel
            {
                Email = "test@example.com",
                Password = "test-pass",
                RememberMe = true
            };
            var user = new User { IsActive = true };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            A.CallTo(() => _authorisationService.ValidateUser(user, "test-pass")).Returns(true);

            var actionResult = _loginController.Post(loginModel);

            A.CallTo(() => _authorisationService.SetAuthCookie("test@example.com", true)).MustHaveHappened();
        }

        [Fact]
        public void LoginController_Post_IfActiveUserIsLoadedIfValidateUserIsTrueReturnsRedirectResult()
        {
            var loginModel = new LoginController.LoginModel
            {
                Email = "test@example.com",
                Password = "test-pass",
                RememberMe = true
            };
            var user = new User { IsActive = true };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            A.CallTo(() => _authorisationService.ValidateUser(user, "test-pass")).Returns(true);

            var actionResult = _loginController.Post(loginModel);

            actionResult.Should().BeOfType<RedirectResult>();
        }

        [Fact]
        public void LoginController_Post_IfActiveUserIsLoadedIfValidateUserIsTrueRedirectsToReturnUrlIfSet()
        {
            var loginModel = new LoginController.LoginModel
            {
                Email = "test@example.com",
                Password = "test-pass",
                RememberMe = true,
                ReturnUrl = "test-redirect"
            };
            var user = new User { IsActive = true };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            A.CallTo(() => _authorisationService.ValidateUser(user, "test-pass")).Returns(true);

            var actionResult = _loginController.Post(loginModel);

            actionResult.As<RedirectResult>().Url.Should().Be("test-redirect");
        }

        [Fact]
        public void LoginController_Post_IfActiveUserIsLoadedIfValidateUserIsTrueRedirectsToAdminRootIfReturnUrlIsNotSet()
        {
            var loginModel = new LoginController.LoginModel
            {
                Email = "test@example.com",
                Password = "test-pass",
                RememberMe = true,
                ReturnUrl = null
            };
            var user = new User { IsActive = true };
            A.CallTo(() => _userService.GetUserByEmail("test@example.com")).Returns(user);
            A.CallTo(() => _authorisationService.ValidateUser(user, "test-pass")).Returns(true);

            var actionResult = _loginController.Post(loginModel);

            actionResult.As<RedirectResult>().Url.Should().Be("~/admin");
        }
    }
}