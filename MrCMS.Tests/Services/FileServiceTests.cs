using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using Xunit;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Tests.Services
{
    public class FileServiceTests : InMemoryDatabaseTest
    {
        private IFileSystem _fileSystem;
        private MediaSettings _mediaSettings;
        private SiteSettings _siteSettings;

        private FileService GetFileService(ISession session = null, IFileSystem fileSystem = null)
        {
            _fileSystem = A.Fake<IFileSystem>();

            _mediaSettings = new MediaSettings
                                {
                                    LargeImageHeight = 480,
                                    LargeImageWidth = 640,
                                    MediumImageHeight = 200,
                                    MediumImageWidth = 320,
                                    SmallImageHeight = 75,
                                    SmallImageWidth = 100,
                                    ThumbnailImageHeight = 64,
                                    ThumbnailImageWidth = 64
                                };
            _siteSettings = new SiteSettings();
            return new FileService(session ?? Session, fileSystem ?? _fileSystem,
                                   A.Fake<IImageProcessor>(), _mediaSettings, CurrentSite, _siteSettings);
        }

        [Fact]
        public void FileService_AddFile_CreatesANewFileRecord()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            Session.QueryOver<MediaFile>().RowCount().Should().Be(1);
        }

        private static MediaCategory GetDefaultMediaCategory()
        {
            return new MediaCategory { Name = "test-category", UrlSegment = "test-category" };
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameNameAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), "test-file.txt", null, 0, mediaCategory);

            mediaFile.FileName.Should().Be("test-file.txt");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentTypeAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            mediaFile.ContentType.Should().Be("text/plain");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentLengthAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.ContentLength.Should().Be(1234);
        }

        [Fact]
        public void FileService_AddFile_ShouldSetFileExtension()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            Stream stream = GetDefaultStream();

            fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = Session.Get<MediaFile>(1);
            file.FileExtension.Should().Be(".txt");
        }

        [Fact]
        public void FileService_AddFile_ShouldSetFileLocation()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            Stream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            A.CallTo(() => _fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain"))
             .Returns("/content/upload/1/test-category/test.txt");

            fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = Session.Get<MediaFile>(1);
            file.FileUrl.Should().Be("/content/upload/1/test-category/test.txt");
        }

        [Fact]
        public void FileService_AddFile_MediaCategoryShouldHaveFile()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));

            fileService.AddFile(GetDefaultStream(), "test1.txt", "text/plain", 1234, mediaCategory);

            mediaCategory.Files.Should().HaveCount(1);
        }

        private static MemoryStream GetDefaultStream()
        {
            return new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }

        [Fact]
        public void FileService_AddFile_CallsSaveToFileSystemOfIFileSystem()
        {
            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));

            Stream stream = GetDefaultStream();

            var fileService = GetFileService();

            fileService.AddFile(stream, "test.txt", "text/plain", 10, mediaCategory);

            A.CallTo(() => _fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain")).MustHaveHappened();
        }

        [Fact]
        public void FileService_GetFileByLocation_ReturnsAMediaFileIfOneIsSavedWithAMatchingLocation()
        {
            var fileService = GetFileService();

            const string fileUrl = "location.jpg";
            var mediaFile = new MediaFile
                                {
                                    FileUrl = fileUrl
                                };
            Session.Transact(session => session.Save(mediaFile));

            var fileByLocation = fileService.GetFileByUrl(fileUrl);

            fileByLocation.Should().Be(mediaFile);
        }
    }
}