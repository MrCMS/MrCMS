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
        public void Logout_CallsAuthorisationServiceLogout()
        {
            var authorisationService = A.Fake<IAuthorisationService>();
            var logoutController = new LogoutControllerBuilder().WithAuthorisationService(authorisationService).Build();

            logoutController.Logout();

            A.CallTo(() => authorisationService.Logout()).MustHaveHappened();
        }

        [Fact]
        public void Logout_RedirectsToRoute()
        {
            var logoutController = new LogoutControllerBuilder().Build();

            var result = logoutController.Logout();

            result.Url.Should().Be("~");
        }
    }
}