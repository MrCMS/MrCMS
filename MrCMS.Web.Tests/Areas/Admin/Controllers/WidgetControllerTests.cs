using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Web.Application.Widgets;
using MrCMS.Web.Areas.Admin.Controllers;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WidgetControllerTests
    {
        private static IWidgetService _widgetService;
        private static IDocumentService _documentService;

        [Fact]
        public void WidgetController_EditGet_ShouldCallWidgetServiceGetWidget()
        {
            var widgetController = GetWidgetController();

            widgetController.Edit(1);

            A.CallTo(() => _widgetService.GetWidget<Widget>(1)).MustHaveHappened();
        }

        private static WidgetController GetWidgetController()
        {
            _documentService = A.Fake<IDocumentService>();
            _widgetService = A.Fake<IWidgetService>();
            var layoutAreaService = A.Fake<ILayoutAreaService>();
            return new WidgetController(_documentService, _widgetService, layoutAreaService, A.Fake<ISession>())
                       {IsAjaxRequest = false};
        }

        [Fact]
        public void WidgetController_EditGet_ShouldReturnTheResultOfTheCallToWidgetService()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget();
            A.CallTo(() => _widgetService.GetWidget<Widget>(1)).Returns(textWidget);

            var result = widgetController.Edit(1);

            result.Model.Should().Be(textWidget);
        }

        [Fact]
        public void WidgetController_EditPost_ShouldCallSaveWidgetOnTheWidgetService()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget { LayoutArea = new LayoutArea() };

            widgetController.Edit(textWidget);

            A.CallTo(() => _widgetService.SaveWidget(textWidget)).MustHaveHappened();
        }

        [Fact]
        public void WidgetController_EditPost_ShouldByDefaultRedirectToLayoutIndex()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget { LayoutArea = new LayoutArea { Id = 1 } };

            var result = widgetController.Edit(textWidget);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("LayoutArea");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_EditPost_IfReturnUrlIsSetRedirectToThere()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            var result = widgetController.Edit(textWidget, "test-url");

            result.As<RedirectResult>().Url.Should().Be("test-url");
        }

        [Fact]
        public void WidgetController_DeleteGet_ReturnsPartialViewResult()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            widgetController.Delete_Get(textWidget).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WidgetController_DeleteGet_ReturnsPassedObjectAsModel()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            widgetController.Delete_Get(textWidget).As<PartialViewResult>().Model.Should().Be(textWidget);
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlRedirectToRouteResult()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            widgetController.Delete(textWidget, null).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WidgetController_DeletePost_IfReturnUrlIsSetReturnsRedirectResult()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            var actionResult = widgetController.Delete(textWidget, "test");
            actionResult.Should().BeOfType<RedirectResult>();
            actionResult.As<RedirectResult>().Url.Should().Be("test");
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlWebpageSetRedirectsToEditWebpage()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget {Webpage = new TextPage {Id = 1}};

            var result = widgetController.Delete(textWidget, null).As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlLayoutAreaIdSetRedirectsToEditLayoutArea()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget {LayoutArea = new LayoutArea {Id = 1}};

            var result = widgetController.Delete(textWidget, null).As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("LayoutArea");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_DeletePost_ReturnUrlContainsWidgetEditIgnoreReturnUrl()
        {
            var widgetController = GetWidgetController();
            var textWidget = new TextWidget {Id = 1, LayoutArea = new LayoutArea {Id = 1}};

            var result = widgetController.Delete(textWidget, "/widget/edit/1").As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("LayoutArea");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_Add_ReturnsPartialViewResult()
        {
            var widgetController = GetWidgetController();

            widgetController.Add(1, null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WidgetController_Add_CallsGetAddWidgetModelWithArguments()
        {
            var widgetController = GetWidgetController();

            widgetController.Add(1, "return-url");

            A.CallTo(() => _widgetService.GetAddWidgetModel(1, "return-url")).MustHaveHappened();
        }
    }
}