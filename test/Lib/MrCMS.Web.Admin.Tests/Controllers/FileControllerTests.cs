using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Controllers;
using MrCMS.Web.Admin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
{
    public class FileControllerTests
    {
        private IFileAdminService _fileAdminService = A.Fake<IFileAdminService>();

        private FileController GetFileController()
        {
            return new FileController(_fileAdminService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request = { Form = new FormCollection(new Dictionary<string, StringValues>()) }
                    }
                }
            };
        }


        [Fact]
        public async Task FileController_Delete_ShouldBeViewResult()
        {
            FileController fileController = GetFileController();

            var delete = await fileController.Delete(0);
            
            delete.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task FileController_Delete_CallsDeleteFileOnFileService()
        {
            FileController fileController = GetFileController();
            await fileController.Delete_POST(1);
            var mediaFile = A.Fake<MediaFile>();
            A.CallTo(() => _fileAdminService.GetFile(1)).Returns(mediaFile);
            A.CallTo(() => _fileAdminService.DeleteFile(1)).MustHaveHappened();
        }

        [Fact]
        public async Task FileController_FilesPost_ReturnsJsonNetResult()
        {
            FileController fileController = GetFileController();

            var filesPost = await fileController.Files_Post(123);
            filesPost.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task FileController_FilesPost_CallsAddFileForTheUploadedFileIfIsValidFile()
        {
            var random = new Random();
            var buffer = Enumerable.Range(1, random.Next(2, 200000))
                .Select(x => (byte)random.Next(0, 255)).ToArray();
            var memoryStream = new MemoryStream();
            memoryStream.Write(buffer, 0, buffer.Length);
            string fileName = "test.txt";
            string contentType = "text/plain";
            int contentLength = buffer.Length;
            A.CallTo(() => _fileAdminService.IsValidFileType("test.txt")).Returns(true);
            FileController fileController = GetFileController();
            fileController.ControllerContext.HttpContext.Request.Form = new FormCollection(
                new Dictionary<string, StringValues>(), new FormFileCollection
                {
                    new FormFile(memoryStream, 0, contentLength, fileName, fileName)
                    {
                        Headers = new HeaderDictionary{["Content-Type"] = contentType }
                    }
                });

            await fileController.Files_Post(123);


            A.CallTo(() => _fileAdminService.AddFile(A<Stream>.That.Matches(x => x.Length == buffer.Length), fileName,
                contentType, contentLength, 123)).MustHaveHappened();
        }

        [Fact]
        public async Task FileController_FilesPost_DoesNotCallAddFileForTheUploadedFileIfIsNotAValidType()
        {
            var random = new Random();
            var buffer = Enumerable.Range(1, random.Next(2, 200000))
                .Select(x => (byte)random.Next(0, 255)).ToArray();
            var memoryStream = new MemoryStream();
            memoryStream.Write(buffer, 0, buffer.Length);
            string fileName = "test.txt";
            string contentType = "text/plain";
            int contentLength = buffer.Length;
            A.CallTo(() => _fileAdminService.IsValidFileType("test.txt")).Returns(false);
            FileController fileController = GetFileController();
            fileController.ControllerContext.HttpContext.Request.Form = new FormCollection(
                new Dictionary<string, StringValues>(), new FormFileCollection
                {
                    new FormFile(memoryStream, 0, contentLength, fileName, fileName)
                    {
                        Headers = new HeaderDictionary{["Content-Type"] = contentType }
                    }
                });
            fileController.ControllerContext.HttpContext.Request.Headers["Content-Type"] = contentType;

            await fileController.Files_Post(123);

            A.CallTo(() => _fileAdminService.AddFile(A<Stream>.That.Matches(x => x.Length == buffer.Length), fileName,
                contentType, contentLength, 123)).MustNotHaveHappened();
        }
    }
}