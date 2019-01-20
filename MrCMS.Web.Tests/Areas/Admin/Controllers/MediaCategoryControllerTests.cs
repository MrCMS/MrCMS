using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
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
            _mediaCategoryController = new MediaCategoryController(_mediaCategoryAdminService,_fileAdminService);
        }

        [Fact]
        public void MediaCategoryController_AddGet_ShouldReturnAMediaCategory()
        {
            var mediaCategory = A.Dummy<MediaCategory>();
            A.CallTo(() => _mediaCategoryAdminService.GetNewCategoryModel(123)).Returns(mediaCategory);
            var actionResult = _mediaCategoryController.Add_Get(123);

            actionResult.Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldCallSaveDocument()
        {
            var mediaCategory = new MediaCategory();

            _mediaCategoryController.Add(mediaCategory);

            A.CallTo(() => _mediaCategoryAdminService.Add(mediaCategory)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldRedirectToShow()
        {
            var mediaCategory = new MediaCategory {Id = 1};

            var result = _mediaCategoryController.Add(mediaCategory) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Show");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnAViewResult()
        {
            ActionResult result = _mediaCategoryController.Edit_Get(new MediaCategory());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var mediaCategory = new MediaCategory {Id = 1};

            var result = _mediaCategoryController.Edit_Get(mediaCategory) as ViewResult;

            result.Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldCallSaveDocument()
        {
            var mediaCategory = new MediaCategory {Id = 1};

            _mediaCategoryController.Edit(mediaCategory);

            A.CallTo(() => _mediaCategoryAdminService.Update(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldRedirectToEdit()
        {
            var mediaCategory = new MediaCategory {Id = 1};

            ActionResult actionResult = _mediaCategoryController.Edit(mediaCategory);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Show");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldBeAListOfSortItems()
        {
            var mediaCategory = new MediaCategory();
            var sortItems = new List<SortItem> {};
            A.CallTo(() => _mediaCategoryAdminService.GetSortItems(mediaCategory)).Returns(sortItems);

            var viewResult = _mediaCategoryController.Sort(mediaCategory).As<ViewResult>();

            viewResult.Model.Should().Be(sortItems);
        }

        [Fact]
        public void MediaCategoryController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = _mediaCategoryController.Index(null);

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void MediaCategoryController_Show_IncorrectCategoryIdRedirectsToIndex()
        {
            ActionResult actionResult = _mediaCategoryController.Show(null);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnAPartialView()
        {
            ActionResult result = _mediaCategoryController.Upload(new MediaCategory());

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnTheResultOfTheMediaCategoryPassedToIt()
        {
            var mediaCategory = new MediaCategory {Name = "test"};

            ActionResult result = _mediaCategoryController.Upload(mediaCategory);

            result.As<PartialViewResult>().Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_RemoveMedia_ShouldReturnAPartialView()
        {
            _mediaCategoryController.RemoveMedia().Should().BeOfType<PartialViewResult>();
        }
    }
}