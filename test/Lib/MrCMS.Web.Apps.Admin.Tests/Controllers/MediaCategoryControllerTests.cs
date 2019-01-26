using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using System.Collections.Generic;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class MediaCategoryControllerTests
    {
        private readonly IFileAdminService _fileAdminService;
        private readonly MediaCategoryController _mediaCategoryController;
        private readonly IMediaCategoryAdminService _mediaCategoryAdminService;

        public MediaCategoryControllerTests()
        {
            _fileAdminService = A.Fake<IFileAdminService>();
            _mediaCategoryAdminService = A.Fake<IMediaCategoryAdminService>();
            _mediaCategoryController = new MediaCategoryController(_mediaCategoryAdminService, _fileAdminService) { TempData = new MockTempDataDictionary() };
        }

        [Fact]
        public void MediaCategoryController_AddGet_ShouldReturnAnAddCategoryModel()
        {
            AddMediaCategoryModel model = new AddMediaCategoryModel();
            A.CallTo(() => _mediaCategoryAdminService.GetNewCategoryModel(123)).Returns(model);

            var result = _mediaCategoryController.Add_Get(123);

            result.Model.Should().Be(model);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldCallAdd()
        {
            var mediaCategory = new AddMediaCategoryModel();

            _mediaCategoryController.Add(mediaCategory);

            A.CallTo(() => _mediaCategoryAdminService.Add(mediaCategory)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldRedirectToShow()
        {
            var model = new AddMediaCategoryModel();
            MediaCategory mediaCategory = new MediaCategory { Id = 123 };
            A.CallTo(() => _mediaCategoryAdminService.Add(model)).Returns(mediaCategory);

            var result = _mediaCategoryController.Add(model);

            result.ActionName.Should().Be("Show");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnAViewResult()
        {
            var result = _mediaCategoryController.Edit_Get(123);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnEditModelAsViewModel()
        {
            var model = new UpdateMediaCategoryModel();
            A.CallTo(() => _mediaCategoryAdminService.GetEditModel(123)).Returns(model);

            var result = _mediaCategoryController.Edit_Get(123);

            result.Model.Should().Be(model);
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldCallUpdate()
        {
            var model = new UpdateMediaCategoryModel { Id = 1 };

            _mediaCategoryController.Edit(model);

            A.CallTo(() => _mediaCategoryAdminService.Update(model)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldRedirectToShow()
        {
            var model = new UpdateMediaCategoryModel { };
            var category = new MediaCategory {Id = 1};
            A.CallTo(() => _mediaCategoryAdminService.Update(model)).Returns(category);

            var result = _mediaCategoryController.Edit(model);

            result.ActionName.Should().Be("Show");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldBeAListOfSortItems()
        {
            var sortItems = new List<SortItem> { };
            A.CallTo(() => _mediaCategoryAdminService.GetSortItems(123)).Returns(sortItems);

            var viewResult = _mediaCategoryController.Sort(123).As<ViewResult>();

            viewResult.Model.Should().Be(sortItems);
        }

        [Fact]
        public void MediaCategoryController_Index_ReturnsViewResult()
        {
            ViewResult result = _mediaCategoryController.Index(null);

            result.Should().NotBeNull();
        }

        [Fact]
        public void MediaCategoryController_Show_IncorrectCategoryIdRedirectsToIndex()
        {
            ActionResult actionResult = _mediaCategoryController.Show(null);

            actionResult.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }
    }
}