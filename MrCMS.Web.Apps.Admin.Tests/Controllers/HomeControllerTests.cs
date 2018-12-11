using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void HomeController_OnGetIndex_ReturnsAViewResult()
        {
            var homeController = new HomeController();

            ActionResult actionResult = homeController.Index();

            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<ViewResult>();
        }
    }
}