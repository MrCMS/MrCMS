using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
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