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
        private static IFileSystem _fileSystem;
        private static MediaSettings mediaSettings;

        [Fact]
        public void FileService_AddFile_NullMediaCategoryThrowsArgumentNullException()
        {
            var fileService = GetFileService();

            Stream stream = GetDefaultStream();
            fileService.Invoking(service => service.AddFile(stream, "test.txt", "text/plain", 10, null)).ShouldThrow
                <ArgumentNullException>();
        }

        private FileService GetFileService(ISession session = null, IFileSystem fileSystem = null)
        {
            _fileSystem = A.Fake<IFileSystem>();

            mediaSettings = new MediaSettings
                                {
                                    LargeImageHeight = 480,
                                    LargeImageWidth = 640,
                                    MediumImageHeight = 200,
                                    MediumImageWidth = 320,
                                    SmallImageHeight = 75,
                                    SmallImageWidth = 100,
                                    ThumbnailImageHeight = 64,
                                    ThumbnailImageWidth = 64,
                                    Site = CurrentSite
                                };
            return new FileService(session ?? Session, fileSystem ?? _fileSystem,
                                   A.Fake<IImageProcessor>(), mediaSettings, CurrentSite);
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

            mediaFile.name.Should().Be("test-file.txt");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentTypeAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            mediaFile.Type.Should().Be("text/plain");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentLengthAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.size.Should().Be(1234);
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
        public void FileService_GetFiles_ShouldReturnFilesWhichHaveTheCorrectMediaCategoryId()
        {
            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var file1 = new MediaFile { MediaCategory = mediaCategory, FileUrl = "/test1.txt" };
            var file2 = new MediaFile { MediaCategory = mediaCategory, FileUrl = "/test2.txt" };
            var file3 = new MediaFile { FileUrl = "/test3.txt" };
            mediaCategory.Files.Add(file1);
            mediaCategory.Files.Add(file2);
            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(file1);
                                     session.SaveOrUpdate(file2);
                                     session.SaveOrUpdate(file3);
                                 });
            var fileService = GetFileService();

            var files = fileService.GetFiles(mediaCategory);

            files.Should().HaveCount(2);

            files.Select(x => x.Id).Should().Contain(file1.Id);
            files.Select(x => x.Id).Should().Contain(file2.Id);
            files.Select(x => x.Id).Should().NotContain(file3.Id);
        }

        [Fact]
        public void FileService_GetFile_ShouldCallSessionGetById()
        {
            var session = A.Fake<ISession>();
            var fileService = GetFileService(session);

            var mediaFile = fileService.GetFile(1);

            A.CallTo(() => session.Get<MediaFile>(1)).MustHaveHappened();
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

        [Fact]
        public void FileService_SaveFile_CallsSessionSaveOrUpdateOnThePassedFile()
        {
            var session = A.Fake<ISession>();
            var fileService = GetFileService(session);
            var mediaFile = new MediaFile();

            fileService.SaveFile(mediaFile);

            A.CallTo(() => session.SaveOrUpdate(mediaFile)).MustHaveHappened();
        }
    }
}