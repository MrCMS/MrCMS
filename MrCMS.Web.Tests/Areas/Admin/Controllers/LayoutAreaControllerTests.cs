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
        private IDocumentService _documentService;
        private ILayoutAreaService _layoutAreaService;

        [Fact]
        public void LayoutAreaController_AddGet_ShouldReturnPartialViewWithAddLayoutAreaModel()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            PartialViewResult partialViewResult = layoutAreaController.Add(1);

            partialViewResult.Model.Should().BeOfType<LayoutArea>();
        }

        private LayoutAreaController GetLayoutAreaController()
        {
            _layoutAreaService = A.Fake<ILayoutAreaService>();
            _documentService = A.Fake<IDocumentService>();
            var layoutAreaController = new LayoutAreaController(_layoutAreaService, _documentService);
            return layoutAreaController;
        }

        [Fact]
        public void LayoutAreaController_AddGet_ShouldSetTheLayoutIdToTheValueFromTheConstructor()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layout = new Layout {Id = 1};
            A.CallTo(() => _documentService.GetDocument<Layout>(1)).Returns(layout);

            PartialViewResult partialViewResult = layoutAreaController.Add(1);

            partialViewResult.Model.As<LayoutArea>().Layout.Should().Be(layout);
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldCallSaveArea()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            layoutAreaController.Add(layoutArea);

            A.CallTo(() => _layoutAreaService.SaveArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldRedirectToEditLayout()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            ActionResult actionResult = layoutAreaController.Add(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_EditGet_LayoutAreaServiceGetLayoutAreaShouldBeCalled()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.Edit(1);

            A.CallTo(() => _layoutAreaService.GetArea(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsValidShouldReturnViewResult()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            ActionResult actionResult = layoutAreaController.Edit(1);

            actionResult.Should().BeOfType<ViewResult>();
            actionResult.As<ViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsInvalidRedirectToLayoutIndex()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(null);

            ActionResult actionResult = layoutAreaController.Edit(1);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldCallLayoutServicesSaveArea()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            layoutAreaController.Edit(layoutArea);

            A.CallTo(() => _layoutAreaService.SaveArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldRedirectBackToTheLayoutOnceDone()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            ActionResult actionResult = layoutAreaController.Edit(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ReturnsAPartialViewResult()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.Delete_Get(null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ShouldReturnObjectPassedAsModel()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();

            layoutAreaController.Delete_Get(layoutArea).As<PartialViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_DeletePost_ShouldCallDeleteAreaForThePassedArea()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = new LayoutArea {Layout = new Layout {Id = 1}};

            layoutAreaController.Delete(layoutArea);

            A.CallTo(() => _layoutAreaService.DeleteArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ShouldCallLayoutAreaServiceGetAreaWithThePassedId()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
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
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            layoutAreaController.SortWidgets(1).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ReturnsGetWidgetsOfArea()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            layoutAreaController.SortWidgets(1).As<ViewResult>().Model.Should().Be(widgets);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsAction_CallsSetWidgetsOrderWithOrdersAsIntList()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsAction("1,2,3");

            A.CallTo(() => _layoutAreaService.SetWidgetOrders("1,2,3")).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_CallsLayoutAreaServiceGetAreaForId()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2);

            A.CallTo(() => _layoutAreaService.GetArea(1)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_CallsDocumentServiceGetWebpageForPageId()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2);

            A.CallTo(() => _documentService.GetDocument<Webpage>(2)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ReturnsViewResult()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelIsPageWidgetSortModel()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            layoutAreaController.SortWidgetsForPage(1, 2)
                                .As<ViewResult>()
                                .Model.Should()
                                .BeOfType<PageWidgetSortModel>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelWidgetsResultOfLayoutAreaGetWidgets()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            var model =
                layoutAreaController.SortWidgetsForPage(1, 2)
                                    .As<ViewResult>()
                                    .Model.As<PageWidgetSortModel>();

            model.Widgets.Should().BeEquivalentTo(widgets);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelAreaShouldBeResultOfLayoutAreaServiceGetArea()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.Id).Returns(1);
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            var model =
                layoutAreaController.SortWidgetsForPage(1, 2)
                                    .As<ViewResult>()
                                    .Model.As<PageWidgetSortModel>();

            model.LayoutAreaId.Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ViewResultModelPageShouldBeResultOFGetDocument()
        {
            LayoutAreaController layoutAreaController = GetLayoutAreaController();

            var document = new TextPage{Id = 1};
            A.CallTo(() => _documentService.GetDocument<Webpage>(2)).Returns(document);

            var model =
                layoutAreaController.SortWidgetsForPage(1, 2)
                                    .As<ViewResult>()
                                    .Model.As<PageWidgetSortModel>();
            
            model.WebpageId.Should().Be(1);
        }
    }
}