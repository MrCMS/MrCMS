using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Services;
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
    }
}