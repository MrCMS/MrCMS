using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class LayoutControllerTests
    {
        private readonly LayoutController _layoutController;
        private readonly ILayoutAdminService _layoutAdminService = A.Fake<ILayoutAdminService>();

        public LayoutControllerTests()
        {
            _layoutController = new LayoutController(_layoutAdminService);
        }

        [Fact]
        public void LayoutController_AddGet_ShouldReturnANewLayoutObject()
        {
            var layout = A.Dummy<Layout>();
            A.CallTo(() => _layoutAdminService.GetAddLayoutModel(123)).Returns(layout);
            var actionResult = _layoutController.Add_Get(123);

            actionResult.Model.Should().Be(layout);
        }


        [Fact]
        public void LayoutController_AddPost_ShouldCallSaveDocument()
        {
            var layout = new Layout();

            _layoutController.Add(layout);

            A.CallTo(() => _layoutAdminService.Add(layout)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void LayoutController_AddPost_ShouldRedirectToView()
        {
            var layout = new Layout {Id = 1};

            var result = _layoutController.Add(layout) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnAViewResult()
        {
            var layout = new Layout {Id = 1};

            ActionResult result = _layoutController.Edit_Get(layout);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void LayoutController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var layout = new Layout {Id = 1};

            var result = _layoutController.Edit_Get(layout) as ViewResult;

            result.Model.Should().Be(layout);
        }

        [Fact]
        public void LayoutController_EditPost_ShouldCallSaveDocument()
        {
            var layout = new Layout {Id = 1};

            _layoutController.Edit(layout);

            A.CallTo(() => _layoutAdminService.Update(layout)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_EditPost_ShouldRedirectToEdit()
        {
            var layout = new Layout {Id = 1};

            ActionResult actionResult = _layoutController.Edit(layout);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }


        [Fact]
        public void LayoutController_Sort_ShouldBeAListOfSortItems()
        {
            var layout = new Layout();
            var sortItems = new List<SortItem> {};
            A.CallTo(() => _layoutAdminService.GetSortItems(layout)).Returns(sortItems);

            var viewResult = _layoutController.Sort(layout).As<ViewResult>();

            viewResult.Model.Should().Be(sortItems);
        }

        [Fact]
        public void LayoutController_View_InvalidIdReturnsRedirectToIndex()
        {
            ActionResult actionResult = _layoutController.Show(null);

            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void LayoutController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = _layoutController.Index();

            actionResult.Should().NotBeNull();
        }
    }
}