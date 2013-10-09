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
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class MediaCategoryControllerTests
    {
        private readonly IDocumentService _documentService;
        private readonly IFileService _fileService;
        private readonly Site _site;
        private readonly MediaCategoryController _mediaCategoryController;

        public MediaCategoryControllerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _fileService = A.Fake<IFileService>();
            _site = new Site();
            _mediaCategoryController = new MediaCategoryController(_documentService, _fileService, _site);
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
        public void MediaCategoryController_AddPost_ShouldRedirectToEdit()
        {
            var mediaCategory = new MediaCategory { Id = 1 };

            var result = _mediaCategoryController.Add(mediaCategory) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
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
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
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
        public void MediaCategoryController_View_ShouldRedirectToEditWithTheSameId()
        {
            ActionResult actionResult = _mediaCategoryController.Show(new MediaCategory { Id = 1 });

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_View_IncorrectCategoryIdRedirectsToIndex()
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
        public void MediaCategoryController_UploadTemplate_ShouldReturnAPartialView()
        {
            ActionResult result = _mediaCategoryController.UploadTemplate();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Thumbnails_ShouldReturnAPartialView()
        {
            ActionResult result = _mediaCategoryController.Thumbnails();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_DownloadTemplate_ShouldReturnAPartialView()
        {
            ActionResult result = _mediaCategoryController.DownloadTemplate();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_RemoveMedia_ShouldReturnAPartialView()
        {
            _mediaCategoryController.RemoveMedia().Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_FileResult_ReturnsAPartialViewResult()
        {
            _mediaCategoryController.FileResult(new MediaFile()).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_FileResult_ReturnsTheMediaFilePassedToIt()
        {
            var mediaFile = new MediaFile();

            _mediaCategoryController.FileResult(mediaFile).Model.Should().Be(mediaFile);
        }

        [Fact]
        public void MediaCategoryController_MiniUploader_ReturnsPartialView()
        {
            _mediaCategoryController.MiniUploader(1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_MiniUploader_ReturnsIdPassedToTheMethod()
        {
            _mediaCategoryController.MiniUploader(1).Model.Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_GetFileUrl_CallsFileServiceGetFileUrlWithPassedValue()
        {
            _mediaCategoryController.GetFileUrl("test");

            A.CallTo(() => _fileService.GetFileUrl("test")).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_GetFileUrl_ReturnsResultOfCallToFileUrl()
        {
            A.CallTo(() => _fileService.GetFileUrl("test")).Returns("test-result");

            _mediaCategoryController.GetFileUrl("test").Should().Be("test-result");
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_ReturnsAPartialViewResult()
        {
            _mediaCategoryController.MediaSelector(null, false, 1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_CallsGetFilesPagedOnFileService()
        {
            _mediaCategoryController.MediaSelector(1, false, 1);

            A.CallTo(() => _fileService.GetFilesPaged(1, false, 1)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_ShouldHaveViewDataSet()
        {
            _mediaCategoryController.MediaSelector(1, false, 1);

            _mediaCategoryController.ViewData["categories"].Should().BeOfType<List<SelectListItem>>();
        }
    }
}