using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Widgets;
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
        public void WidgetController_EditGet_ShouldReturnThePassedWidget()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget {Site = new Site()};

            ViewResultBase result = widgetController.Edit_Get(textWidget);

            result.Model.Should().Be(textWidget);
        }

        [Fact]
        public void WidgetController_EditPost_ShouldCallSaveWidgetOnTheWidgetService()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget {LayoutArea = new LayoutArea()};

            widgetController.Edit(textWidget);

            A.CallTo(() => _widgetService.SaveWidget(textWidget)).MustHaveHappened();
        }

        [Fact]
        public void WidgetController_EditPost_ShouldByDefaultRedirectToLayoutIndex()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget {LayoutArea = new LayoutArea {Id = 1}};

            ActionResult result = widgetController.Edit(textWidget);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("LayoutArea");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_EditPost_IfReturnUrlIsSetRedirectToThere()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            ActionResult result = widgetController.Edit(textWidget, "test-url");

            result.As<RedirectResult>().Url.Should().Be("test-url");
        }

        [Fact]
        public void WidgetController_DeleteGet_ReturnsPartialViewResult()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            widgetController.Delete_Get(textWidget).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void WidgetController_DeleteGet_ReturnsPassedObjectAsModel()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            widgetController.Delete_Get(textWidget).As<PartialViewResult>().Model.Should().Be(textWidget);
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlRedirectToRouteResult()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            widgetController.Delete(textWidget, null).Should().BeOfType<RedirectToRouteResult>();
        }

        [Fact]
        public void WidgetController_DeletePost_IfReturnUrlIsSetReturnsRedirectResult()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget();

            ActionResult actionResult = widgetController.Delete(textWidget, "test");
            actionResult.Should().BeOfType<RedirectResult>();
            actionResult.As<RedirectResult>().Url.Should().Be("test");
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlWebpageSetRedirectsToEditWebpage()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget {Webpage = new TextPage {Id = 1}};

            var result = widgetController.Delete(textWidget, null).As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("Webpage");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_DeletePost_NullReturnUrlLayoutAreaIdSetRedirectsToEditLayoutArea()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget {LayoutArea = new LayoutArea {Id = 1}};

            var result = widgetController.Delete(textWidget, null).As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("LayoutArea");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void WidgetController_DeletePost_ReturnUrlContainsWidgetEditIgnoreReturnUrl()
        {
            WidgetController widgetController = GetWidgetController();
            var textWidget = new TextWidget {Id = 1, LayoutArea = new LayoutArea {Id = 1}};

            var result = widgetController.Delete(textWidget, "/widget/edit/1").As<RedirectToRouteResult>();

            result.RouteValues["controller"].Should().Be("LayoutArea");
            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        //[Fact]
        //public void WidgetController_AddGet_ReturnsPartialViewResult()
        //{
        //    WidgetController widgetController = GetWidgetController();

        //    widgetController.Add(new LayoutArea(), null).Should().BeOfType<PartialViewResult>();
        //}

        //[Fact]
        //public void WidgetController_Add_ReturnsAnAddWidgetModel()
        //{
        //    WidgetController widgetController = GetWidgetController();
        //    var layoutArea = new LayoutArea();
            
        //    var result = widgetController.Add(layoutArea, "return-url");

        //    result.Model.Should().BeOfType<AddWidgetModel>();
        //}

        private static WidgetController GetWidgetController()
        {
            _documentService = A.Fake<IDocumentService>();
            _widgetService = A.Fake<IWidgetService>();
            return new WidgetController(_documentService, _widgetService, A.Fake<ISession>()) {ReferrerOverride = "http://www.example.com/"};
        }
    }
}