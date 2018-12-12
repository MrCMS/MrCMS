using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Web.Apps.Admin.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class WebpageWidgetControllerTests
    {
        private readonly IWebpageWidgetAdminService _webpageWidgetAdminService;
        private readonly WebpageWidgetController _webpageWidgetController;

        public WebpageWidgetControllerTests()
        {
            _webpageWidgetAdminService = A.Fake<IWebpageWidgetAdminService>();
            _webpageWidgetController = new WebpageWidgetController(_webpageWidgetAdminService);
        }

        [Fact]
        public void WebpageWidgetController_Hide_CallsServiceHideWithPassedArguments()
        {
            _webpageWidgetController.Hide(123, 2, 3);

            A.CallTo(() => _webpageWidgetAdminService.Hide(123, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageWidgetController_Hide_ReturnsARedirectToRouteResult()
        {
            _webpageWidgetController.Hide(123, 2, 3).Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void WebpageWidgetController_Hide_SetsRouteValuesForIdAndLayoutAreaId()
        {

            var result = _webpageWidgetController.Hide(123, 2, 3);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
            result.RouteValues["layoutAreaId"].Should().Be(3);
        }

        [Fact]
        public void WebpageWidgetController_Show_CallsServiceShowWithPassedArguments()
        {
            _webpageWidgetController.Show(123, 2, 3);

            A.CallTo(() => _webpageWidgetAdminService.Show(123, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageWidgetController_Show_ReturnsARedirectToRouteResult()
        {
            _webpageWidgetController.Show(123, 2, 3).Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void WebpageWidgetController_Show_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var redirectToRouteResult = _webpageWidgetController.Show(123, 2, 3).As<RedirectToActionResult>();

            redirectToRouteResult.ActionName.Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(123);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }
    }
}