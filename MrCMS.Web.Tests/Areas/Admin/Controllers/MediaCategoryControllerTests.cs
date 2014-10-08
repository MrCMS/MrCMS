using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class MediaCategoryControllerTests
    {
        private readonly IDocumentService _documentService;
        private readonly IFileAdminService _fileService;
        private readonly MediaCategoryController _mediaCategoryController;
        private IUrlValidationService _urlValidationService;

        public MediaCategoryControllerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _fileService = A.Fake<IFileAdminService>();
            _urlValidationService = A.Fake<IUrlValidationService>();
            _mediaCategoryController = new MediaCategoryController(_fileService, _documentService, _urlValidationService);
        }

        [Fact]
        public void MediaCategoryController_AddGet_ShouldReturnAMediaCategory()
        {
            var actionResult = _mediaCategoryController.Add_Get(null) as ViewResult;

            actionResult.Model.Should().BeOfType<MediaCategory>();
        }

        [Fact]
        public void MediaCategoryController_AddGet_ShouldSetParentOfModelToModelInMethod()
        {
            var mediaCategory = new MediaCategory { Id = 1 };
            A.CallTo(() => _documentService.GetDocument<MediaCategory>(1)).Returns(mediaCategory);

            var actionResult = _mediaCategoryController.Add_Get(1) as ViewResult;

            actionResult.Model.As<MediaCategory>().Parent.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldCallSaveDocument()
        {
            var mediaCategory = new MediaCategory();

            _mediaCategoryController.Add(mediaCategory);

            A.CallTo(() => _documentService.AddDocument(mediaCategory)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldRedirectToShow()
        {
            var mediaCategory = new MediaCategory { Id = 1 };

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
            var mediaCategory = new MediaCategory { Id = 1 };

            var result = _mediaCategoryController.Edit_Get(mediaCategory) as ViewResult;

            result.Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldCallSaveDocument()
        {
            var mediaCategory = new MediaCategory { Id = 1 };

            _mediaCategoryController.Edit(mediaCategory);

            A.CallTo(() => _documentService.SaveDocument(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldRedirectToEdit()
        {
            var mediaCategory = new MediaCategory { Id = 1 };

            ActionResult actionResult = _mediaCategoryController.Edit(mediaCategory);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Show");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldCallGetDocumentsByParent()
        {
            var mediaCategory = new MediaCategory();
            _mediaCategoryController.Sort(mediaCategory);

            A.CallTo(() => _documentService.GetDocumentsByParent(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldBeAListOfSortItems()
        {
            var mediaCategory = new MediaCategory();
            var mediaCategories = new List<MediaCategory> { new MediaCategory() };
            A.CallTo(() => _documentService.GetDocumentsByParent(mediaCategory)).Returns(mediaCategories);

            var viewResult = _mediaCategoryController.Sort(mediaCategory).As<ViewResult>();

            viewResult.Model.Should().BeOfType<List<SortItem>>();
        }

        [Fact]
        public void MediaCategoryController_Index_ReturnsViewResult()
        {
            ViewResult actionResult = _mediaCategoryController.Index();

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
            var mediaCategory = new MediaCategory { Name = "test" };

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