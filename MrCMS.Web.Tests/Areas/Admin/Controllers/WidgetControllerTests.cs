using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WidgetControllerTests : MrCMSTest
    {
        private readonly WidgetController _widgetController;
        private readonly IWidgetService _widgetService;
        private InMemoryRepository<Webpage> _webpageRepository = new InMemoryRepository<Webpage>();

        public WidgetControllerTests()
        {
            Kernel.Bind<ISetWidgetAdminViewData>().ToConstant(A.Fake<ISetWidgetAdminViewData>());
            _widgetService = A.Fake<IWidgetService>();
            _widgetController = new WidgetController(_webpageRepository, _widgetService)
            {
                ReferrerOverride = "http://www.example.com/"
            };
        }

        [Fact]
        public void WidgetController_EditGet_ShouldReturnThePassedWidget()
        {
            var textWidget = new TextWidget {Site = new Site()};

            ViewResultBase result = _widgetController.Edit_Get(textWidget);

            result.Model.Should().Be(textWidget);
        }

        [Fact]
        public void WidgetController_EditPost_ShouldCallSaveWidgetOnTheWidgetService()
        {
            var textWidget = new TextWidget {LayoutArea = new LayoutArea()};

            _widgetController.Edit(textWidget);

            A.CallTo(() => _widgetService.SaveWidget(textWidget)).MustHaveHappened();
        }

        [Fact]
        public void WidgetController_EditPost_ShouldByDefaultRedirectToLayoutIndex()
        {
            var textWidget = new TextWidget {LayoutArea = new LayoutArea {Id = 1}};

            ActionResult result = _widgetController.Edit(textWidget);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("LayoutArea");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_EditPost_IfReturnUrlIsSetRedirectToThere()
        {
            var textWidget = new TextWidget();

            ActionResult result = _widgetController.Edit(textWidget, "test-url");

            result.As<RedirectResult>().Url.Should().Be("test-url");
        }

        [Fact]
        public void WidgetController_DeleteGet_ReturnsPartialViewResult()
        {
            var textWidget = new TextWidget();

            _widgetController.Delete_Get(textWidget).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WidgetController_DeleteGet_ReturnsPassedObjectAsModel()
        {
            var textWidget = new TextWidget();

            _widgetController.Delete_Get(textWidget).As<PartialViewResult>().Model.Should().Be(textWidget);
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlRedirectToRouteResult()
        {
            var textWidget = new TextWidget();

            _widgetController.Delete(textWidget, null).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WidgetController_DeletePost_IfReturnUrlIsSetReturnsRedirectResult()
        {
            var textWidget = new TextWidget();

            ActionResult actionResult = _widgetController.Delete(textWidget, "test");
            actionResult.Should().BeOfType<RedirectResult>();
            actionResult.As<RedirectResult>().Url.Should().Be("test");
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlWebpageSetRedirectsToEditWebpage()
        {
            var textWidget = new TextWidget {Webpage = new TextPage {Id = 1}};

            var result = _widgetController.Delete(textWidget, null).As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlLayoutAreaIdSetRedirectsToEditLayoutArea()
        {
            var textWidget = new TextWidget {LayoutArea = new LayoutArea {Id = 1}};

            var result = _widgetController.Delete(textWidget, null).As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("LayoutArea");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_DeletePost_ReturnUrlContainsWidgetEditIgnoreReturnUrl()
        {
            var textWidget = new TextWidget {Id = 1, LayoutArea = new LayoutArea {Id = 1}};

            var result = _widgetController.Delete(textWidget, "/widget/edit/1").As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("LayoutArea");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }
    }
}