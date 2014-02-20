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
            var homeController = new HomeController();

            ActionResult actionResult = homeController.Index();

            actionResult.Should().NotBeNull();
            actionResult.Should().BeOfType<ViewResult>();
        }
    }
}