using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Tests.Website.Controllers.Builders;
using Xunit;

namespace MrCMS.Tests.Website.Controllers
{
    public class LogoutControllerTests
    {
        [Fact]
        public async Task Logout_CallsAuthorisationServiceLogout()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var logoutController = new LogoutControllerBuilder().WithAuthorisationService(authorisationService).Build();

            await logoutController.Logout();

            A.CallTo(() => authorisationService.Logout()).MustHaveHappened();
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