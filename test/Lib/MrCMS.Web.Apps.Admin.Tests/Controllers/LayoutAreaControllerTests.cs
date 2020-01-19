using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
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
            var addLayoutAreaModel = new AddLayoutAreaModel();
            A.CallTo(() => _layoutAreaAdminService.GetAddModel(123)).Returns(addLayoutAreaModel);

            PartialViewResult partialViewResult = _layoutAreaController.Add(123);

            partialViewResult.Model.Should().Be(addLayoutAreaModel);
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldCallSaveArea()
        {
            var layoutAreaModel = new AddLayoutAreaModel();

            _layoutAreaController.Add(layoutAreaModel);

            A.CallTo(() => _layoutAreaAdminService.Add(layoutAreaModel)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_AddPost_ShouldRedirectToEditLayout()
        {
            var layoutAreaModel = new AddLayoutAreaModel { LayoutId = 123 };

            var actionResult = _layoutAreaController.Add(layoutAreaModel);

            actionResult.ActionName.Should().Be("Edit");
            actionResult.ControllerName.Should().Be("Layout");
            actionResult.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsValidShouldReturnViewResult()
        {
            var model = new UpdateLayoutAreaModel();
            A.CallTo(() => _layoutAreaAdminService.GetEditModel(123)).Returns(model);

            ActionResult actionResult = _layoutAreaController.Edit_Get(123);

            actionResult.Should().BeOfType<ViewResult>();
            actionResult.As<ViewResult>().Model.Should().Be(model);
        }

        [Fact]
        public void LayoutAreaController_EditGet_IfIdIsInvalidRedirectToLayoutIndex()
        {
            A.CallTo(() => _layoutAreaAdminService.GetEditModel(123)).Returns(null);
            ActionResult actionResult = _layoutAreaController.Edit_Get(123);

            actionResult.Should().BeOfType<RedirectToActionResult>();
            actionResult.As<RedirectToActionResult>().ActionName.Should().Be("Index");
            actionResult.As<RedirectToActionResult>().ControllerName.Should().Be("Layout");
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldCallLayoutServicesSaveArea()
        {
            var model = new UpdateLayoutAreaModel();

            _layoutAreaController.Edit(model);

            A.CallTo(() => _layoutAreaAdminService.Update(model)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_EditPost_ShouldRedirectBackToTheLayoutOnceDone()
        {
            var model = new UpdateLayoutAreaModel();
            var layoutArea = new LayoutArea();
            var layout = new Layout { Id = 123 };
            layoutArea.Layout = layout;
            A.CallTo(() => _layoutAreaAdminService.Update(model)).Returns(layoutArea);

            var actionResult = _layoutAreaController.Edit(model);

            actionResult.ActionName.Should().Be("Edit");
            actionResult.ControllerName.Should().Be("Layout");
            actionResult.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ReturnsAPartialViewResult()
        {

            _layoutAreaController.Delete_Get(123).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void LayoutAreaController_Delete_Get_ShouldReturnModelFromServiceAsViewModel()
        {
            var model = new UpdateLayoutAreaModel();
            A.CallTo(() => _layoutAreaAdminService.GetEditModel(123)).Returns(model);

            var result = _layoutAreaController.Delete_Get(123);

            result.Model.Should().Be(model);
        }

        [Fact]
        public void LayoutAreaController_DeletePost_ShouldCallDeleteAreaForThePassedArea()
        {

            _layoutAreaController.Delete(123);

            A.CallTo(() => _layoutAreaAdminService.DeleteArea(123)).MustHaveHappened();
        }

        [Fact]
        public void LayoutAreaController_SortWidgetsForPage_ReturnsViewResult()
        {
            _layoutAreaController.SortWidgetsForPage(123, 2).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task LayoutAreaController_SortWidgetsForPage_IsResultOfCallToGetSortModel()
        {
            var newLayoutArea = GetNewLayoutArea();
            var pageWidgetSortModel = new PageWidgetSortModel();
            A.CallTo(() => _layoutAreaAdminService.GetSortModel(newLayoutArea, 2)).Returns(pageWidgetSortModel);
            A.CallTo(() => _layoutAreaAdminService.GetArea(123)).Returns(newLayoutArea);

            var result = (await _layoutAreaController.SortWidgetsForPage(123, 2));

            result.Model.Should().Be(pageWidgetSortModel);
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