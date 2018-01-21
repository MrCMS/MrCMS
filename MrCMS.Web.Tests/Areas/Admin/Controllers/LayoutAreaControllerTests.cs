using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutAreaControllerTests
    {
        private ILayoutAreaAdminService _layoutAreaAdminService;
        private LayoutAreaController _layoutAreaController;

        public LayoutAreaControllerTests()
        {
            _layoutAreaAdminService = A.Fake<ILayoutAreaAdminService>();
            _layoutAreaController = new LayoutAreaController(_layoutAreaAdminService);
        }

        [Fact]
        public void LayoutAreaController_AddGet_ShouldReturnPartialViewWithAddLayoutAreaModel()
        {
            PartialViewResult partialViewResult = _layoutAreaController.Add(new Layout());

            partialViewResult.Model.Should().BeOfType<LayoutArea>();
        }

        [Fact]
        public void LayoutAreaController_AddGet_ShouldSetTheLayoutIdToTheValueFromTheConstructor()
        {
            var layout = new Layout();

            PartialViewResult partialViewResult = _layoutAreaController.Add(layout);

            partialViewResult.Model.As<LayoutArea>().Layout.Should().Be(layout);
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldCallSaveArea()
        {
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            _layoutAreaController.Add(layoutArea);

            A.CallTo(() => _layoutAreaAdminService.Add(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldRedirectToEditLayout()
        {
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            ActionResult actionResult = _layoutAreaController.Add(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsValidShouldReturnViewResult()
        {
            var layoutArea = new LayoutArea();

            ActionResult actionResult = _layoutAreaController.Edit_Get(layoutArea);

            actionResult.Should().BeOfType<ViewResult>();
            actionResult.As<ViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsInvalidRedirectToLayoutIndex()
        {
            ActionResult actionResult = _layoutAreaController.Edit_Get(null);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldCallLayoutServicesSaveArea()
        {
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            _layoutAreaController.Edit(layoutArea);

            A.CallTo(() => _layoutAreaAdminService.Update(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldRedirectBackToTheLayoutOnceDone()
        {
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 1 };
            layoutArea.Layout = layout;

            ActionResult actionResult = _layoutAreaController.Edit(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ReturnsAPartialViewResult()
        {

            _layoutAreaController.Delete_Get(null).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ShouldReturnObjectPassedAsModel()
        {
            var layoutArea = new LayoutArea();

            _layoutAreaController.Delete_Get(layoutArea).As<PartialViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void LayoutAreaController_DeletePost_ShouldCallDeleteAreaForThePassedArea()
        {
            var layoutArea = new LayoutArea { Layout = new Layout { Id = 1 } };

            _layoutAreaController.Delete(layoutArea);

            A.CallTo(() => _layoutAreaAdminService.DeleteArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ReturnsAViewResult()
        {
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);

            _layoutAreaController.SortWidgets(layoutArea).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgets_ReturnsGetWidgetsOfArea()
        {
            var layoutArea = A.Fake<LayoutArea>();
            var widgets = new List<Widget>();
            A.CallTo(() => layoutArea.GetWidgets(null, false)).Returns(widgets);

            _layoutAreaController.SortWidgets(layoutArea)
                                .Model.As<PageWidgetSortModel>()
                                .Widgets.Should()
                                .BeEquivalentTo(widgets);
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ReturnsViewResult()
        {
            _layoutAreaController.SortWidgetsForPage(GetNewLayoutArea(), 2).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_IsResultOfCallToGetSortModel()
        {
            var newLayoutArea = GetNewLayoutArea();
            var pageWidgetSortModel = new PageWidgetSortModel();
            A.CallTo(() => _layoutAreaAdminService.GetSortModel(newLayoutArea, 2)).Returns(pageWidgetSortModel);
            _layoutAreaController.SortWidgetsForPage(newLayoutArea, 2)
                .Model.Should()
                .Be(pageWidgetSortModel);
        }

        private LayoutArea GetNewLayoutArea()
        {
            return new FakeLayoutArea();
        }
    }

    internal class FakeLayoutArea : LayoutArea
    {
        public FakeLayoutArea()
        {
            Widgets = new List<Widget>();
        }
    }
}