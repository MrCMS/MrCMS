using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class LayoutControllerTests
    {
        private readonly LayoutController _layoutController;
        private readonly ILayoutAdminService _layoutAdminService = A.Fake<ILayoutAdminService>();

        public LayoutControllerTests()
        {
            _layoutController = new LayoutController(_layoutAdminService) { TempData = new MockTempDataDictionary() };
        }

        [Fact]
        public void LayoutController_AddGet_ShouldReturnAnAddLayoutModel()
        {
            var model = new AddLayoutModel();
            A.CallTo(() => _layoutAdminService.GetAddLayoutModel(123)).Returns(model);
            var actionResult = _layoutController.Add_Get(123);

            actionResult.Model.Should().Be(model);
        }


        [Fact]
        public async Task LayoutController_AddPost_ShouldCallSaveDocument()
        {
            var model = new AddLayoutModel();

            await _layoutController.Add(model);

            A.CallTo(() => _layoutAdminService.Add(model)).MustHaveHappened();
        }

        [Fact]
        public async Task LayoutController_AddPost_ShouldRedirectToEdit()
        {
            var layout = new Layout { Id = 123 };
            var model = new AddLayoutModel();
            A.CallTo(() => _layoutAdminService.Add(model)).Returns(layout);

            var result = await _layoutController.Add(model);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }


        [Fact]
        public async Task LayoutController_EditGet_ShouldReturnUpdateModelAsModel()
        {
            var model = new UpdateLayoutModel();
            A.CallTo(() => _layoutAdminService.GetEditModel(123)).Returns(model);

            var result = await _layoutController.Edit_Get(123);

            result.Model.Should().Be(model);
        }

        [Fact]
        public void LayoutController_EditPost_ShouldCallUpdate()
        {
            var model = new UpdateLayoutModel { Id = 1 };

            _layoutController.Edit(model);

            A.CallTo(() => _layoutAdminService.Update(model)).MustHaveHappened();
        }

        [Fact]
        public void LayoutController_EditPost_ShouldRedirectToEdit()
        {
            var model = new UpdateLayoutModel { Id = 1 };

            var result = _layoutController.Edit(model);

            result.ActionName.Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }


        [Fact]
        public void LayoutController_Sort_ShouldBeAListOfSortItems()
        {
            var sortItems = new List<SortItem> { };
            A.CallTo(() => _layoutAdminService.GetSortItems(123)).Returns(sortItems);

            var viewResult = _layoutController.Sort(123);

            viewResult.Model.Should().Be(sortItems);
        }


        [Fact]
        public void LayoutController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = _layoutController.Index();

            actionResult.Should().NotBeNull();
        }
    }
}