using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutAreaControllerTests
    {
        private ILayoutAreaService _layoutAreaService;
        private IDocumentService _documentService;

        [Fact]
        public void ZoneController_AddGet_ShouldReturnPartialViewWithAddLayoutAreaModel()
        {
            var zoneController = GetLayoutAreaController();

            var partialViewResult = zoneController.Add(1);

            partialViewResult.Model.Should().BeOfType<LayoutArea>();
        }

        private LayoutAreaController GetLayoutAreaController()
        {
            _layoutAreaService = A.Fake<ILayoutAreaService>();
            _documentService = A.Fake<IDocumentService>();
            var zoneController = new LayoutAreaController(_layoutAreaService, _documentService) {IsAjaxRequest = false};
            return zoneController;
        }

        [Fact]
        public void ZoneController_AddGet_ShouldSetTheLayoutIdToTheValueFromTheConstructor()
        {
            var zoneController = GetLayoutAreaController();
            var layout = new Layout {Id = 1};
            A.CallTo(() => _documentService.GetDocument<Layout>(1)).Returns(layout);

            var partialViewResult = zoneController.Add(1);

            partialViewResult.Model.As<LayoutArea>().Layout.Should().Be(layout);
        }

        [Fact]
        public void ZoneController_AddPost_ShouldCallSaveArea()
        {
            var zoneController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            zoneController.Add(layoutArea);

            A.CallTo(() => _layoutAreaService.SaveArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void ZoneController_AddPost_ShouldRedirectToEditLayout()
        {
            var zoneController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            var actionResult = zoneController.Add(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void ZoneController_EditGet_LayoutAreaServiceGetLayoutAreaShouldBeCalled()
        {
            var zoneController = GetLayoutAreaController();

            zoneController.Edit(1);

            A.CallTo(() => _layoutAreaService.GetArea(1)).MustHaveHappened();
        }

        [Fact]
        public void ZoneController_EditGet_IfIdIsValidShouldReturnViewResult()
        {
            var zoneController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(layoutArea);

            var actionResult = zoneController.Edit(1);

            actionResult.Should().BeOfType<ViewResult>();
            actionResult.As<ViewResult>().Model.Should().Be(layoutArea);
        }

        [Fact]
        public void ZoneController_EditGet_IfIdIsInvalidRedirectToLayoutIndex()
        {
            var zoneController = GetLayoutAreaController();
            A.CallTo(() => _layoutAreaService.GetArea(1)).Returns(null);

            var actionResult = zoneController.Edit(1);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
        }

        [Fact]
        public void ZoneController_EditPost_ShouldCallLayoutServicesSaveArea()
        {
            var zoneController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            zoneController.Edit(layoutArea);

            A.CallTo(() => _layoutAreaService.SaveArea(layoutArea)).MustHaveHappened();
        }

        [Fact]
        public void ZoneController_EditPost_ShouldRedirectBackToTheLayoutOnceDone()
        {
            var zoneController = GetLayoutAreaController();
            var layoutArea = new LayoutArea();
            var layout = new Layout {Id = 1};
            layoutArea.Layout = layout;

            var actionResult = zoneController.Edit(layoutArea);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["controller"].Should().Be("Layout");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }
    }
}