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
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Website.ActionResults;
using MrCMS.Website.Controllers;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class FileControllerTests
    {
        private static IFileAdminService fileAdminService;

        private static FileController GetFileController()
        {
            fileAdminService = A.Fake<IFileAdminService>();
            var fileController = new FileController(fileAdminService);
            return fileController;
        }

        [Fact]
        public void FileController_UpdateSEO_ShouldReturnChangesSaved()
        {
            FileController fileController = GetFileController();

            var mediaFile = new MediaFile();
            fileController.UpdateSEO(mediaFile, "test", "test").Should().Be("Changes saved");
        }

        [Fact]
        public void FileController_UpdateSEO_ShouldSetTitleAndDescriptionFromInputs()
        {
            FileController fileController = GetFileController();

            var mediaFile = new MediaFile();
            fileController.UpdateSEO(mediaFile, "test-title", "test-description");

            mediaFile.Title.Should().Be("test-title");
            mediaFile.Description.Should().Be("test-description");
        }

        [Fact]
        public void FileController_UpdateSEO_IfErrorIsThrownOnSaveReturnValueContainsMessage()
        {
            FileController fileController = GetFileController();

            var mediaFile = new MediaFile();
            A.CallTo(() => fileAdminService.SaveFile(mediaFile)).Throws(new Exception("Test exception"));
            fileController.UpdateSEO(mediaFile, "test-title", "test-description")
                          .Should()
                          .Be("There was an error saving the SEO values: Test exception");
        }

        [Fact]
        public void FileController_Delete_ShouldBeViewResult()
        {
            FileController fileController = GetFileController();

            fileController.Delete(new MediaFile()).Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void FileController_Delete_CallsDeleteFileOnFileService()
        {
            FileController fileController = GetFileController();
            var mediaFile = new MediaFile();
            mediaFile.MediaCategory = new MediaCategory{Id = 1};
            fileController.Delete_POST(mediaFile);

            A.CallTo(() => fileAdminService.DeleteFile(mediaFile)).MustHaveHappened();
        }

        [Fact]
        public void FileController_Files_ReturnsAJsonNetResult()
        {
            FileController fileController = GetFileController();

            fileController.Files(new MediaCategory()).Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void FileController_Files_CallsFileServiceGetFilesWithPassedId()
        {
            FileController fileController = GetFileController();
            var mediaCategory = new MediaCategory();
            
            fileController.Files(mediaCategory);

            A.CallTo(() => fileAdminService.GetFiles(mediaCategory)).MustHaveHappened();
        }

        [Fact]
        public void FileController_FilesPost_ReturnsJsonNetResult()
        {
            FileController fileController = GetFileController();
            fileController.RequestMock = A.Fake<HttpRequestBase>();

            fileController.Files_Post(new MediaCategory()).Should().BeOfType<JsonNetResult>();
        }

        [Fact]
        public void FileController_FilesPost_CallsAddFileForTheUploadedFileIfIsValidFile()
        {
            FileController fileController = GetFileController();
            var httpRequestBase = A.Fake<HttpRequestBase>();
            var memoryStream = new MemoryStream();
            string fileName = "test.txt";
            string contentType = "text/plain";
            int contentLength = 0;
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
            A.CallTo(() => fileAdminService.IsValidFileType("test.txt")).Returns(true);
            fileController.RequestMock = httpRequestBase;

            var mediaCategory = new MediaCategory();
            fileController.Files_Post(mediaCategory);

            A.CallTo(() => fileAdminService.AddFile(memoryStream, fileName, contentType, contentLength, mediaCategory))
             .MustHaveHappened();
        }

        [Fact]
        public void FileController_FilesPost_DoesNotCallAddFileForTheUploadedFileIfIsNotAValidType()
        {
            FileController fileController = GetFileController();
            var httpRequestBase = A.Fake<HttpRequestBase>();
            var memoryStream = new MemoryStream();
            string fileName = "test.txt";
            string contentType = "text/plain";
            int contentLength = 0;
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
            A.CallTo(() => fileAdminService.IsValidFileType("test.txt")).Returns(false);
            fileController.RequestMock = httpRequestBase;

            var mediaCategory = new MediaCategory();
            fileController.Files_Post(mediaCategory);

            A.CallTo(() => fileAdminService.AddFile(memoryStream, fileName, contentType, contentLength, mediaCategory))
                .MustNotHaveHappened();
        }
    }
}