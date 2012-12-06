using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
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
        private IImageProcessor _imageProcessor;
        private ISitesService _sitesService;

        [Fact]
        public void MediaCategoryController_AddGet_ShouldReturnAddPageModel()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var actionResult = mediaCategoryController.Add(1) as ViewResult;

            actionResult.Model.Should().BeOfType<AddPageModel>();
        }

        [Fact]
        public void MediaCategoryController_AddGet_ShouldSetParentIdOfModelToIdInMethod()
        {
            var mediaCategoryController = GetMediaCategoryController();
            A.CallTo(() => _documentService.GetDocument<Document>(1)).Returns(new MediaCategory { Id = 1 });

            var actionResult = mediaCategoryController.Add(1) as ViewResult;

            (actionResult.Model as AddPageModel).ParentId.Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldCallSaveDocument()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory();
            mediaCategoryController.Add(mediaCategory);

            A.CallTo(() => _documentService.AddDocument(mediaCategory)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void MediaCategoryController_AddPost_ShouldRedirectToEdit()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory { Id = 1 };
            var result = mediaCategoryController.Add(mediaCategory) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnAViewResult()
        {
            var mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Edit_Get(new MediaCategory());

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };

            var result = mediaCategoryController.Edit_Get(mediaCategory) as ViewResult;

            result.Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldCallSaveDocument()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };

            mediaCategoryController.Edit(mediaCategory);

            A.CallTo(() => _documentService.SaveDocument(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_EditPost_ShouldRedirectToEdit()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };

            ActionResult actionResult = mediaCategoryController.Edit(mediaCategory);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            (actionResult as RedirectToRouteResult).RouteValues["action"].Should().Be("Edit");
            (actionResult as RedirectToRouteResult).RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldCallGetDocumentsByParentId()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.Sort(1);

            A.CallTo(() => _documentService.GetDocumentsByParentId<MediaCategory>(1)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldUseTheResultOfDocumentsByParentIdsAsModel()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategories = new List<MediaCategory> { new MediaCategory() };
            A.CallTo(() => _documentService.GetDocumentsByParentId<MediaCategory>(1)).Returns(mediaCategories);

            var viewResult = mediaCategoryController.Sort(1).As<ViewResult>();

            viewResult.Model.As<List<MediaCategory>>().Should().BeEquivalentTo(mediaCategories);
        }

        [Fact]
        public void MediaCategoryController_SortAction_ShouldCallSortOrderOnTheDocumentServiceWithTheRelevantValues()
        {
            var mediaCategoryController = GetMediaCategoryController();
            mediaCategoryController.SortAction(1, 2);

            A.CallTo(() => _documentService.SetOrder(1, 2)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_Index_ReturnsViewResult()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var actionResult = mediaCategoryController.Index();

            actionResult.Should().NotBeNull();
        }

        [Fact]
        public void MediaCategoryController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.SuggestDocumentUrl(1, "test");

            A.CallTo(() => _documentService.GetDocumentUrl("test", 1, false)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var mediaCategoryController = GetMediaCategoryController();

            A.CallTo(() => _documentService.GetDocumentUrl("test", 1, false)).Returns("test/result");
            var url = mediaCategoryController.SuggestDocumentUrl(1, "test");

            url.Should().BeEquivalentTo("test/result");
        }

        [Fact]
        public void MediaCategoryController_View_ShouldRedirectToEditWithTheSameId()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var actionResult = mediaCategoryController.Show(new MediaCategory { Id = 1 });

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_View_IncorrectCategoryIdRedirectsToIndex()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var actionResult = mediaCategoryController.Show(null);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnAPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Upload(new MediaCategory());

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnTheResultOfTheMediaCategoryPassedToIt()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory { Name = "test" };

            ActionResult result = mediaCategoryController.Upload(mediaCategory);

            result.As<PartialViewResult>().Model.Should().Be(mediaCategory);
        }

        [Fact]
        public void MediaCategoryController_UploadTemplate_ShouldReturnAPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.UploadTemplate();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Thumbnails_ShouldReturnAPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Thumbnails();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_DownloadTemplate_ShouldReturnAPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.DownloadTemplate();

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_RemoveMedia_ShouldReturnAPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.RemoveMedia().Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_FileResult_ReturnsAPartialViewResult()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.FileResult(new MediaFile()).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_FileResult_ReturnsTheMediaFilePassedToIt()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var mediaFile = new MediaFile();

            mediaCategoryController.FileResult(mediaFile).Model.Should().Be(mediaFile);
        }

        [Fact]
        public void MediaCategoryController_MiniUploader_ReturnsPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MiniUploader(1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_MiniUploader_ReturnsIdPassedToTheMethod()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MiniUploader(1).Model.Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_GetFileUrl_CallsFileServiceGetFileUrlWithPassedValue()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.GetFileUrl("test");

            A.CallTo(() => _fileService.GetFileUrl("test")).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_GetFileUrl_ReturnsResultOfCallToFileUrl()
        {
            var mediaCategoryController = GetMediaCategoryController();

            A.CallTo(() => _fileService.GetFileUrl("test")).Returns("test-result");

            mediaCategoryController.GetFileUrl("test").Should().Be("test-result");
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_ReturnsAPartialViewResult()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MediaSelector(null, false, 1).Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_CallsGetFilesPagedOnFileService()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MediaSelector(1, false, 1);

            A.CallTo(() => _fileService.GetFilesPaged(1, false, 1, 10)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_MediaSelector_ShouldHaveViewDataSet()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.MediaSelector(1, false, 1);

            mediaCategoryController.ViewData["categories"].Should().BeOfType<List<SelectListItem>>();
        }

        private MediaCategoryController GetMediaCategoryController()
        {
            _documentService = A.Fake<IDocumentService>();
            _sitesService = A.Fake<ISitesService>();
            _fileService = A.Fake<IFileService>();
            _imageProcessor = A.Fake<IImageProcessor>();
            var mediaCategoryController = new MediaCategoryController(_documentService, _sitesService, _fileService) { IsAjaxRequest = false };
            return mediaCategoryController;
        }
    }
}