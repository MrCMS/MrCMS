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
        private IDocumentService _documentService;
        private IFileService _fileService;

        [Fact]
        public void MediaCategoryController_AddGet_ShouldReturnAMediaCategory()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            var actionResult = mediaCategoryController.Add_Get(null) as ViewResult;

            actionResult.Model.Should().BeOfType<MediaCategory>();
        }

        [Fact]
        public void MediaCategoryController_AddGet_ShouldSetParentOfModelToModelInMethod()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };
            A.CallTo(() => _documentService.GetDocument<MediaCategory>(1)).Returns(mediaCategory);

            var actionResult = mediaCategoryController.Add_Get(1) as ViewResult;

            actionResult.Model.As<MediaCategory>().Parent.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldCallSaveDocument()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory();
            mediaCategoryController.Add(mediaCategory);

            A.CallTo(() => _documentService.AddDocument(mediaCategory)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldRedirectToEdit()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory { Id = 1 };
            var result = mediaCategoryController.Add(mediaCategory) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnAViewResult()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Edit_Get(new MediaCategory());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };

            var result = mediaCategoryController.Edit_Get(mediaCategory) as ViewResult;

            result.Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldCallSaveDocument()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };

            mediaCategoryController.Edit(mediaCategory);

            A.CallTo(() => _documentService.SaveDocument(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldRedirectToEdit()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };

            ActionResult actionResult = mediaCategoryController.Edit(mediaCategory);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldCallGetDocumentsByParent()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory();
            mediaCategoryController.Sort(mediaCategory);

            A.CallTo(() => _documentService.GetDocumentsByParent(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldBeAListOfSortItems()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory();
            var mediaCategories = new List<MediaCategory> { new MediaCategory() };
            A.CallTo(() => _documentService.GetDocumentsByParent(mediaCategory)).Returns(mediaCategories);

            var viewResult = mediaCategoryController.Sort(mediaCategory).As<ViewResult>();

            viewResult.Model.Should().BeOfType<List<SortItem>>();
        }

        [Fact]
        public void MediaCategoryController_Index_ReturnsViewResult()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ViewResult actionResult = mediaCategoryController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void MediaCategoryController_View_ShouldRedirectToEditWithTheSameId()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult actionResult = mediaCategoryController.Show(new MediaCategory { Id = 1 });

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_View_IncorrectCategoryIdRedirectsToIndex()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult actionResult = mediaCategoryController.Show(null);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnAPartialView()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Upload(new MediaCategory());

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnTheResultOfTheMediaCategoryPassedToIt()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory { Name = "test" };

            ActionResult result = mediaCategoryController.Upload(mediaCategory);

            result.As<PartialViewResult>().Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_UploadTemplate_ShouldReturnAPartialView()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.UploadTemplate();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Thumbnails_ShouldReturnAPartialView()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Thumbnails();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_DownloadTemplate_ShouldReturnAPartialView()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.DownloadTemplate();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_RemoveMedia_ShouldReturnAPartialView()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.RemoveMedia().Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_FileResult_ReturnsAPartialViewResult()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.FileResult(new MediaFile()).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_FileResult_ReturnsTheMediaFilePassedToIt()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            var mediaFile = new MediaFile();

            mediaCategoryController.FileResult(mediaFile).Model.Should().Be(mediaFile);
        }

        [Fact]
        public void MediaCategoryController_MiniUploader_ReturnsPartialView()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MiniUploader(1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_MiniUploader_ReturnsIdPassedToTheMethod()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MiniUploader(1).Model.Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_GetFileUrl_CallsFileServiceGetFileUrlWithPassedValue()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.GetFileUrl("test");

            A.CallTo(() => _fileService.GetFileUrl("test")).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_GetFileUrl_ReturnsResultOfCallToFileUrl()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            A.CallTo(() => _fileService.GetFileUrl("test")).Returns("test-result");

            mediaCategoryController.GetFileUrl("test").Should().Be("test-result");
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_ReturnsAPartialViewResult()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MediaSelector(null, false, 1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_CallsGetFilesPagedOnFileService()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MediaSelector(1, false, 1);

            A.CallTo(() => _fileService.GetFilesPaged(1, false, 1, 10)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_ShouldHaveViewDataSet()
        {
            MediaCategoryController mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MediaSelector(1, false, 1);

            mediaCategoryController.ViewData["categories"].Should().BeOfType<List<SelectListItem>>();
        }

        private MediaCategoryController GetMediaCategoryController()
        {
            _documentService = A.Fake<IDocumentService>();
            _fileService = A.Fake<IFileService>();
            var mediaCategoryController = new MediaCategoryController(_documentService, _fileService);
            return mediaCategoryController;
        }
    }
}