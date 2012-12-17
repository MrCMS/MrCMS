using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Application.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutAreaControllerTests
    {
        private ILayoutAreaService _layoutAreaService;
        private IDocumentService _documentService;

        [Fact]
        public void LayoutAreaController_AddGet_ShouldReturnPartialViewWithAddLayoutAreaModel()
        {
            var layoutAreaController = GetLayoutAreaController();

            var partialViewResult = layoutAreaController.Add(1);

            partialViewResult.Model.Should().BeOfType<LayoutArea>();
        }

        private LayoutAreaController GetLayoutAreaController()
        {
            _layoutAreaService = A.Fake<ILayoutAreaService>();
            _documentService = A.Fake<IDocumentService>();
            var layoutAreaController = new LayoutAreaController(_layoutAreaService, _documentService) { IsAjaxRequest = false };
            return layoutAreaController;
        }

        [Fact]
        public void LayoutAreaController_AddGet_ShouldSetTheLayoutIdToTheValueFromTheConstructor()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layout = new Layout { Id = 1 };
            A.CallTo(() => _documentService.GetDocument<Layout>(1)).Returns(layout);

            var partialViewResult = layoutAreaController.Add(1);

            partialViewResult.Model.As<LayoutArea>().Layout.Should().Be(layout);
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldCallSaveArea()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            layoutAreaController.Add(layoutArea);

            A.CallTo(() => _layoutAreaService.SaveArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldRedirectToEditLayout()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            var actionResult = layoutAreaController.Add(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_EditGet_LayoutAreaServiceGetLayoutAreaShouldBeCalled()
        {
            var layoutAreaController = GetLayoutAreaController();

            layoutAreaController.Edit(1);

            A.CallTo(() => _layoutAreaService.GetArea(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsValidShouldReturnViewResult()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            var actionResult = layoutAreaController.Edit(1);

            actionResult.Should().BeOfType<ViewResult>();
            actionResult.As<ViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsInvalidRedirectToLayoutIndex()
        {
            var layoutAreaController = GetLayoutAreaController();
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(null);

            var actionResult = layoutAreaController.Edit(1);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldCallLayoutServicesSaveArea()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            layoutAreaController.Edit(layoutArea);

            A.CallTo(() => _layoutAreaService.SaveArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldRedirectBackToTheLayoutOnceDone()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            var actionResult = layoutAreaController.Edit(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ReturnsAPartialViewResult()
        {
            var layoutAreaController = GetLayoutAreaController();

            layoutAreaController.Delete_Get(null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ShouldReturnObjectPassedAsModel()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();

            layoutAreaController.Delete_Get(layoutArea).As<PartialViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_DeletePost_ShouldCallDeleteAreaForThePassedArea()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea { Layout = new Layout { Id = 1 } };

            layoutAreaController.Delete(layoutArea);

            A.CallTo(() => _layoutAreaService.DeleteArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ShouldCallLayoutAreaServiceGetAreaWithThePassedId()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            layoutAreaController.SortWidgets(1);

            A.CallTo(() => _layoutAreaService.GetArea(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ReturnsAViewResult()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            layoutAreaController.SortWidgets(1).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ReturnsGetWidgetsOfArea()
        {
            var layoutAreaController = GetLayoutAreaController();
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            layoutAreaController.SortWidgets(1).As<ViewResult>().Model.Should().Be(widgets);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsAction_CallsSetWidgetsOrderWithOrdersAsIntList()
        {
            var layoutAreaController = GetLayoutAreaController();
            
            layoutAreaController.SortWidgetsAction("1,2,3");

            A.CallTo(() => _layoutAreaService.SetWidgetOrders("1,2,3")).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_CallsLayoutAreaServiceGetAreaForId()
        {
            var layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2);

            A.CallTo(() => _layoutAreaService.GetArea(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_CallsDocumentServiceGetWebpageForPageId()
        {
            var layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2);

            A.CallTo(() => _documentService.GetDocument<Webpage>(2)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ReturnsViewResult()
        {
            var layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelIsPageWidgetSortModel()
        {
            var layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2)
                                .As<ViewResult>()
                                .Model.Should()
                                .BeOfType<LayoutAreaController.PageWidgetSortModel>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelWidgetsResultOfLayoutAreaGetWidgets()
        {
            var layoutAreaController = GetLayoutAreaController();

            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            var model =
                layoutAreaController.SortWidgetsForPage(1, 2)
                                    .As<ViewResult>()
                                    .Model.As<LayoutAreaController.PageWidgetSortModel>();

            model.Widgets.Should().BeEquivalentTo(widgets);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelAreaShouldBeResultOfLayoutAreaServiceGetArea()
        {
            var layoutAreaController = GetLayoutAreaController();

            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            var model =
                layoutAreaController.SortWidgetsForPage(1, 2)
                                    .As<ViewResult>()
                                    .Model.As<LayoutAreaController.PageWidgetSortModel>();

            model.Area.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelPageShouldBeResultOFGetDocument()
        {
            var layoutAreaController = GetLayoutAreaController();

            var document = new TextPage();
            A.CallTo(() => _documentService.GetDocument<Webpage>(2)).Returns(document);

            var model =
                layoutAreaController.SortWidgetsForPage(1, 2)
                                    .As<ViewResult>()
                                    .Model.As<LayoutAreaController.PageWidgetSortModel>();

            model.Webpage.Should().Be(document);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPageAction_CallsLayoutAreaServiceSetWidgetForPageOrderWithWidgetPageOrder()
        {
            var layoutAreaController = GetLayoutAreaController();

            var widgetPageOrder = new WidgetPageOrder();
            layoutAreaController.SortWidgetsForPageAction(widgetPageOrder);

            A.CallTo(() => _layoutAreaService.SetWidgetForPageOrder(widgetPageOrder));
        }
    }
}