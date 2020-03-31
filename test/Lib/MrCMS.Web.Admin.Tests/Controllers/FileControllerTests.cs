namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class FileControllerTests
    {
        // todo - rewrite tests and refactor

        //private IFileAdminService _fileAdminService = A.Fake<IFileAdminService>();

        //private FileController GetFileController()
        //{
        //    return new FileController(_fileAdminService)
        //    {
        //        ControllerContext = new ControllerContext
        //        {
        //            HttpContext = new DefaultHttpContext
        //            {
        //                Request = { Form = new FormCollection(new Dictionary<string, StringValues>()) }
        //            }
        //        }
        //    };
        //}

        //[Fact]
        //public void FileController_UpdateSEO_ShouldReturnChangesSaved()
        //{
        //    FileController fileController = GetFileController();

        //    var mediaFile = new MediaFile();
        //    fileController.UpdateSEO(mediaFile, "test", "test").Should().Be("Changes saved");
        //}

        //[Fact]
        //public void FileController_UpdateSEO_ShouldSetTitleAndDescriptionFromInputs()
        //{
        //    FileController fileController = GetFileController();

        //    var mediaFile = new MediaFile();
        //    fileController.UpdateSEO(mediaFile, "test-title", "test-description");

        //    mediaFile.Title.Should().Be("test-title");
        //    mediaFile.Description.Should().Be("test-description");
        //}

        //[Fact]
        //public void FileController_UpdateSEO_IfErrorIsThrownOnSaveReturnValueContainsMessage()
        //{
        //    FileController fileController = GetFileController();

        //    var mediaFile = new MediaFile();
        //    A.CallTo(() => _fileAdminService.UpdateFile(mediaFile)).Throws(new Exception("Test exception"));
        //    fileController.UpdateSEO(mediaFile, "test-title", "test-description")
        //                  .Should()
        //                  .Be("There was an error saving the SEO values: Test exception");
        //}

        //[Fact]
        //public void FileController_Delete_ShouldBeViewResult()
        //{
        //    FileController fileController = GetFileController();

        //    fileController.Delete(new MediaFile()).Should().BeOfType<ViewResult>();
        //}

        //[Fact]
        //public void FileController_Delete_CallsDeleteFileOnFileService()
        //{
        //    FileController fileController = GetFileController();
        //    fileController.Delete_POST(1);
        //    var mediaFile = A.Fake<MediaFile>();
        //    A.CallTo(() => _fileAdminService.GetFile(1)).Returns(mediaFile);
        //    // todo implement
        //    //A.CallTo(() => _fileAdminService.DeleteFile(mediaFile)).MustHaveHappened();
        //}

        //[Fact]
        //public void FileController_FilesPost_ReturnsJsonNetResult()
        //{
        //    FileController fileController = GetFileController();

        //    fileController.Files_Post(123).Should().BeOfType<JsonResult>();
        //}

        //[Fact]
        //public async Task FileController_FilesPost_CallsAddFileForTheUploadedFileIfIsValidFile()
        //{
        //    var random = new Random();
        //    var buffer = Enumerable.Range(1, random.Next(2, 200000))
        //        .Select(x => (byte)random.Next(0, 255)).ToArray();
        //    var memoryStream = new MemoryStream();
        //    memoryStream.Write(buffer, 0, buffer.Length);
        //    string fileName = "test.txt";
        //    string contentType = "text/plain";
        //    int contentLength = buffer.Length;
        //    A.CallTo(() => _fileAdminService.IsValidFileType("test.txt")).Returns(true);
        //    FileController fileController = GetFileController();
        //    fileController.ControllerContext.HttpContext.Request.Form = new FormCollection(
        //        new Dictionary<string, StringValues>(), new FormFileCollection
        //        {
        //            new FormFile(memoryStream, 0, contentLength, fileName, fileName)
        //            {
        //                Headers = new HeaderDictionary{["Content-Type"] = contentType }
        //            }
        //        });

        //    await fileController.Files_Post(123);


        //    A.CallTo(() => _fileAdminService.AddFile(A<Stream>.That.Matches(x => x.Length == buffer.Length), fileName,
        //        contentType, contentLength, 123)).MustHaveHappened();
        //}

        //[Fact]
        //public void FileController_FilesPost_DoesNotCallAddFileForTheUploadedFileIfIsNotAValidType()
        //{
        //    var random = new Random();
        //    var buffer = Enumerable.Range(1, random.Next(2, 200000))
        //        .Select(x => (byte)random.Next(0, 255)).ToArray();
        //    var memoryStream = new MemoryStream();
        //    memoryStream.Write(buffer, 0, buffer.Length);
        //    string fileName = "test.txt";
        //    string contentType = "text/plain";
        //    int contentLength = buffer.Length;
        //    A.CallTo(() => _fileAdminService.IsValidFileType("test.txt")).Returns(false);
        //    FileController fileController = GetFileController();
        //    fileController.ControllerContext.HttpContext.Request.Form = new FormCollection(
        //        new Dictionary<string, StringValues>(), new FormFileCollection
        //        {
        //            new FormFile(memoryStream, 0, contentLength, fileName, fileName)
        //            {
        //                Headers = new HeaderDictionary{["Content-Type"] = contentType }
        //            }
        //        });
        //    fileController.ControllerContext.HttpContext.Request.Headers["Content-Type"] = contentType;

        //    fileController.Files_Post(123);

        //    A.CallTo(() => _fileAdminService.AddFile(A<Stream>.That.Matches(x => x.Length == buffer.Length), fileName,
        //        contentType, contentLength, 123)).MustNotHaveHappened();
        //}
    }
}