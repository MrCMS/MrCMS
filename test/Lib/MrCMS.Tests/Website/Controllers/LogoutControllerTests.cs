using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services.Auth;
using MrCMS.Tests.Website.Controllers.Builders;
using Xunit;

namespace MrCMS.Tests.Website.Controllers
{
    public class LogoutControllerTests
    {
        [Fact]
        public async Task Logout_CallsAuthorisationServiceLogout()
        {
            var signInManager = A.Fake<ISignInManager>();
            var logoutController = new LogoutControllerBuilder().WithSignInManager(signInManager).Build();

            await logoutController.Logout();

            A.CallTo(() => signInManager.SignOutAsync()).MustHaveHappened();
        }

        [Fact]
        public async Task Logout_RedirectsToRoute()
        {
            var logoutController = new LogoutControllerBuilder().Build();

            var result = await logoutController.Logout();

            result.Url.Should().Be("~/");
        }
    }
}