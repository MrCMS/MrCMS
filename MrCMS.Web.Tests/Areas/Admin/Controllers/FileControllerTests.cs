using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class FileControllerTests
    {
        private static IFileService fileService;
        private static IDocumentService documentService;

        private static FileController GetFileController()
        {
            fileService = A.Fake<IFileService>();
            documentService = A.Fake<IDocumentService>();
            var fileController = new FileController(fileService, documentService);
            return fileController;
        }

        [Fact]
        public void FileController_UpdateSEO_ShouldReturnChangesSaved()
        {
            var fileController = GetFileController();

            var mediaFile = new MediaFile();
            fileController.UpdateSEO(mediaFile, "test", "test").Should().Be("Changes saved");
        }

        [Fact]
        public void FileController_UpdateSEO_ShouldSetTitleAndDescriptionFromInputs()
        {
            var fileController = GetFileController();

            var mediaFile = new MediaFile();
            fileController.UpdateSEO(mediaFile, "test-title", "test-description");

            mediaFile.Title.Should().Be("test-title");
            mediaFile.Description.Should().Be("test-description");
        }

        [Fact]
        public void FileController_UpdateSEO_IfErrorIsThrownOnSaveReturnValueContainsMessage()
        {
            var fileController = GetFileController();

            var mediaFile = new MediaFile();
            A.CallTo(() => fileService.SaveFile(mediaFile)).Throws(new Exception("Test exception"));
            fileController.UpdateSEO(mediaFile, "test-title", "test-description")
                          .Should()
                          .Be("There was an error saving the SEO values: Test exception");
        }

        [Fact]
        public void FileController_Delete_CallsDeleteFileOnFileService()
        {
            var fileController = GetFileController();

            var mediaFile = new MediaFile();
            fileController.Delete(mediaFile);

            A.CallTo(() => fileService.DeleteFile(mediaFile)).MustHaveHappened();
        }

        [Fact]
        public void FileController_Files_ReturnsAJsonResult()
        {
            var fileController = GetFileController();

            fileController.Files(1).Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void FileController_Files_CallsFileServiceGetFilesWithPassedId()
        {
            var fileController = GetFileController();

            fileController.Files(1);

            A.CallTo(() => fileService.GetFiles(1)).MustHaveHappened();
        }

        [Fact]
        public void FileController_FilesPost_ReturnsJsonResult()
        {
            var fileController = GetFileController();
            fileController.RequestMock = A.Fake<HttpRequestBase>();

            fileController.Files_Post(1).Should().BeOfType<JsonResult>();
        }

        [Fact]
        public void FileController_FilesPost_CallsAddFileForTheUploadedFile()
        {
            var fileController = GetFileController();
            var httpRequestBase = A.Fake<HttpRequestBase>();
            var memoryStream = new MemoryStream();
            var fileName = "test.txt";
            var contentType = "text/plain";
            var contentLength = 0;
            var httpFileCollectionWrapper =
                new FakeHttpFileCollectionBase(new Dictionary<string, HttpPostedFileBase>
                                                   {
                                                       {
                                                           "test",
                                                           new FakeHttpPostedFileBase(memoryStream, fileName,
                                                                                      contentType, contentLength)
                                                       }
                                                   });
            A.CallTo(() => httpRequestBase.Files).Returns(httpFileCollectionWrapper);
            fileController.RequestMock = httpRequestBase;

            var mediaCategory = new MediaCategory();
            A.CallTo(() => documentService.GetDocument<MediaCategory>(1)).Returns(mediaCategory);
            fileController.Files_Post(1);

            A.CallTo(() => fileService.AddFile(memoryStream, fileName, contentType, contentLength, mediaCategory))
             .MustHaveHappened();
        }
    }
}