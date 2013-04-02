using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void HomeController_OnGetIndex_ReturnsAViewResult()
        {
            var siteService = A.Fake<ISiteService>();
            var userService = A.Fake<IUserService>();
            var session = A.Fake<ISession>();
            var homeController = new HomeController(siteService, userService, session);

            ActionResult actionResult = homeController.Index();

            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<ViewResult>();
        }
    }
}