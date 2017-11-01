using System.Threading.Tasks;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Models.Auth;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Services.Resources;
using MrCMS.Web.Apps.Core.Controllers;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;
using Xunit;

namespace MrCMS.Web.Tests.Controllers
{
    public class LoginControllerTests : MrCMSTest
    {
        //private readonly LoginController _loginController;
        //private readonly ILoginService _loginService;
        //private readonly IResetPasswordService _resetPasswordService;
        //private readonly IUniquePageService _uniquePageService;
        //private readonly IUserLookup _userService;

        //public LoginControllerTests()
        //{
        //    _userService = A.Fake<IUserLookup>();
        //    _resetPasswordService = A.Fake<IResetPasswordService>();
        //    _loginService = A.Fake<ILoginService>();
        //    _uniquePageService = A.Fake<IUniquePageService>();
        //    _loginController = new LoginController(_resetPasswordService, _uniquePageService,
        //        _loginService, A.Fake<IStringResourceProvider>(), _userService);

        //    // initial setup as this is reused
        //    A.CallTo(() => _uniquePageService.RedirectTo<LoginPage>(null)).Returns(new RedirectResult("~/login-page"));
        //    A.CallTo(() => _uniquePageService.RedirectTo<ForgottenPasswordPage>(null))
        //        .Returns(new RedirectResult("~/forgotten-password"));
        //}

        //[Fact]
        //public void LoginController_Show_ShouldReturnLoginPageAsModel()
        //{
        //    var result = _loginController.Show(new LoginPage(), new LoginModel());

        //    result.Model.Should().BeOfType<LoginPage>();
        //}

        //[Fact]
        //public void LoginController_Show_ShouldSetViewDataAsModel()
        //{
        //    var loginModel = new LoginModel();

        //    var result = _loginController.Show(null, loginModel);

        //    result.ViewData["login-model"].Should().Be(loginModel);
        //}

        //[Fact]
        //public void LoginController_Show_ShouldUseTempDataModelBeforePassedModel()
        //{
        //    var model1 = new LoginModel();
        //    var model2 = new LoginModel();
        //    _loginController.TempData["login-model"] = model1;

        //    var result = _loginController.Show(null, model2);

        //    result.ViewData["login-model"].Should().Be(model1);
        //}

        //[Fact]
        //public void LoginController_Post_IfModelIsNullReturnsRedirectToLoginPage()
        //{
        //    var actionResult = _loginController.Post(null).Result;

        //    actionResult.Should().NotBeNull();
        //    actionResult.Url.Should().Be("~/login-page");
        //}

        //[Fact]
        //public void LoginController_Post_IfModelIsNotNullButModelStateIsInvalidReturnRedirectToLoginPage()
        //{
        //    _loginController.ModelState.AddModelError("test", "error");
        //    var actionResult = _loginController.Post(new LoginModel()).Result;

        //    actionResult.Should().NotBeNull();
        //    actionResult.Url.Should().Be("~/login-page");
        //}

        //[Fact]
        //public void LoginController_Post_IfModelIsNotNullButModelStateIsInvalidShouldNotCallAuthenticateUser()
        //{
        //    _loginController.ModelState.AddModelError("test", "error");
        //    var loginModel = new LoginModel();
        //    _loginController.Post(loginModel).GetAwaiter().GetResult();

        //    A.CallTo(() => _loginService.AuthenticateUser(loginModel)).MustNotHaveHappened();
        //}

        //[Fact]
        //public void LoginController_Post_IfAuthenticateUserReturnsSuccessShouldRedirectToReturnedUrl()
        //{
        //    var loginModel = new LoginModel();
        //    A.CallTo(() => _loginService.AuthenticateUser(loginModel))
        //        .Returns(Task.Run(() => new LoginResult {Status = LoginStatus.Success, RedirectUrl = "redirect-url"}));
        //    var redirectResult = _loginController.Post(loginModel).Result;

        //    redirectResult.Url.Should().Be("redirect-url");
        //}

        //[Fact]
        //public void LoginController_Post_IfAuthenticateUserReturnsFailureRedirectToLoginPage()
        //{
        //    var loginModel = new LoginModel();
        //    A.CallTo(() => _loginService.AuthenticateUser(loginModel))
        //        .Returns(Task.Run(() => new LoginResult {Status = LoginStatus.Failure}));

        //    var redirectResult = _loginController.Post(loginModel).Result;

        //    redirectResult.Url.Should().Be("~/login-page");
        //}

        //[Fact]
        //public void LoginController_Post_IfAuthenticateUserReturnsFailureShouldSetMessageToModel()
        //{
        //    var loginModel = new LoginModel();
        //    A.CallTo(() => _loginService.AuthenticateUser(loginModel))
        //        .Returns(Task.Run(() => new LoginResult { Status = LoginStatus.Failure, Message = "failure message"}));

        //    var redirectResult = _loginController.Post(loginModel).Result;

        //    loginModel.Message.Should().Be("failure message");
        //}


        //[Fact]
        //public void LoginController_ForgottenPasswordGET_ShouldReturnAView()
        //{
        //    _loginController.ForgottenPassword("").Should().NotBeNull();
        //}

        //[Fact]
        //public void LoginController_ForgottenPasswordPOST_ShouldCallGetUserByEmailWithPassedEmail()
        //{
        //    var forgottenPassword = _loginController.ForgottenPassword("test@example.com");

        //    A.CallTo(() => _userService.GetUserByEmail("test@example.com")).MustHaveHappened();
        //}
    }
}