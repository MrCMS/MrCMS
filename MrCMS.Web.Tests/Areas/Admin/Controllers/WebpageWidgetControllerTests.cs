using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
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
        public void WebpageWidgetController_Hide_CallsDocumentServiceHideWithPassedArguments()
        {
            var stubWebpage = new StubWebpage();

            _webpageWidgetController.Hide(stubWebpage, 2, 3);

            A.CallTo(() => _webpageWidgetAdminService.Hide(stubWebpage, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageWidgetController_Hide_ReturnsARedirectToRouteResult()
        {
            var stubWebpage = new StubWebpage();

            _webpageWidgetController.Hide(stubWebpage, 2, 3).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageWidgetController_Hide_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var stubWebpage = new StubWebpage {Id = 1};

            var redirectToRouteResult = _webpageWidgetController.Hide(stubWebpage, 2, 3).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(stubWebpage.Id);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }

        [Fact]
        public void WebpageWidgetController_Show_CallsDocumentServiceShowWithPassedArguments()
        {
            var stubWebpage = new StubWebpage();

            _webpageWidgetController.Show(stubWebpage, 2, 3);

            A.CallTo(() => _webpageWidgetAdminService.Show(stubWebpage, 2)).MustHaveHappened();
        }

        [Fact]
        public void WebpageWidgetController_Show_ReturnsARedirectToRouteResult()
        {
            var stubWebpage = new StubWebpage();

            _webpageWidgetController.Show(stubWebpage, 2, 3).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WebpageWidgetController_Show_SetsRouteValuesForIdAndLayoutAreaId()
        {
            var stubWebpage = new StubWebpage {Id = 1};

            var redirectToRouteResult = _webpageWidgetController.Show(stubWebpage, 2, 3).As<RedirectToRouteResult>();

            redirectToRouteResult.RouteValues["action"].Should().Be("Edit");
            redirectToRouteResult.RouteValues["id"].Should().Be(stubWebpage.Id);
            redirectToRouteResult.RouteValues["layoutAreaId"].Should().Be(3);
        }
    }
}