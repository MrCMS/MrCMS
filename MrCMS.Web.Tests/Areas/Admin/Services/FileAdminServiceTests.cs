using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class FileAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly FileAdminService _fileAdminService;
        private IFileService _fileService;

        public FileAdminServiceTests()
        {
            _fileService = A.Fake<IFileService>();
            _fileAdminService = new FileAdminService(_fileService, Session);
        }

        [Fact]
        public void FileService_GetFiles_ShouldReturnFilesWhichHaveTheCorrectMediaCategoryId()
        {
            MediaCategory mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var file1 = new MediaFile {MediaCategory = mediaCategory, FileUrl = "/test1.txt"};
            var file2 = new MediaFile {MediaCategory = mediaCategory, FileUrl = "/test2.txt"};
            var file3 = new MediaFile {FileUrl = "/test3.txt"};
            mediaCategory.Files.Add(file1);
            mediaCategory.Files.Add(file2);
            Session.Transact(session =>
            {
                session.SaveOrUpdate(file1);
                session.SaveOrUpdate(file2);
                session.SaveOrUpdate(file3);
            });

            ViewDataUploadFilesResult[] files = _fileAdminService.GetFiles(mediaCategory);

            files.Should().HaveCount(2);

            files.Select(x => x.Id).Should().Contain(file1.Id);
            files.Select(x => x.Id).Should().Contain(file2.Id);
            files.Select(x => x.Id).Should().NotContain(file3.Id);
        }

        private static MediaCategory GetDefaultMediaCategory()
        {
            return new MediaCategory {Name = "test-category", UrlSegment = "test-category"};
        }
    }
}