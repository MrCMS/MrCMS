using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FakeItEasy.Configuration;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
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

            ActionResult result = mediaCategoryController.Edit(1);

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MediaCategoryController_EditGet_ShouldReturnLayoutAsViewModel()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory { Id = 1 };
            A.CallTo(() => _documentService.GetDocument<MediaCategory>(1)).Returns(mediaCategory);

            var result = mediaCategoryController.Edit(1) as ViewResult;

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

            A.CallTo(() => _documentService.GetAdminDocumentsByParentId<MediaCategory>(1)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_Sort_ShouldUseTheResultOfDocumentsByParentIdsAsModel()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategories = new List<MediaCategory> { new MediaCategory() };
            A.CallTo(() => _documentService.GetAdminDocumentsByParentId<MediaCategory>(1)).Returns(mediaCategories);

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

            var actionResult = mediaCategoryController.View(1);

            actionResult.Should().BeOfType<RedirectToRouteResult>();
            actionResult.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            actionResult.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(1);
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldCallGetMediaCategoryForTheGivenId()
        {
            var mediaCategoryController = GetMediaCategoryController();

            mediaCategoryController.Upload(1);

            A.CallTo(() => _documentService.GetDocument<MediaCategory>(1)).MustHaveHappened();
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnAPartialView()
        {
            var mediaCategoryController = GetMediaCategoryController();

            ActionResult result = mediaCategoryController.Upload(1);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MediaCategoryController_Upload_ShouldReturnTheResultOfTheMediaCategoryLookup()
        {
            var mediaCategoryController = GetMediaCategoryController();

            var mediaCategory = new MediaCategory { Name = "test" };
            A.CallTo(() => _documentService.GetDocument<MediaCategory>(1)).Returns(mediaCategory);

            ActionResult result = mediaCategoryController.Upload(1);

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
        public void MediaCategoryController_Media_ReturnsCategoryIfPassedToMethod()
        {
            var mediaCategoryController = GetMediaCategoryController();
            var mediaCategory = new MediaCategory();

            ActionResult result = mediaCategoryController.Media(mediaCategory, null);

            result.As<PartialViewResult>().Model.Should().Be(mediaCategory);
        }

        private MediaCategoryController GetMediaCategoryController()
        {
            _documentService = A.Fake<IDocumentService>();
            _fileService = A.Fake<IFileService>();
            _imageProcessor = A.Fake<IImageProcessor>();
            var mediaCategoryController = new MediaCategoryController(_documentService, _fileService, _imageProcessor) { IsAjaxRequest = false };
            return mediaCategoryController;
        }
    }
}