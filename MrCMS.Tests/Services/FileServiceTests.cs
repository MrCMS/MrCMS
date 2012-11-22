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
        [Fact]
        public void FileService_AddFile_NullMediaCategoryThrowsArgumentNullException()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            Stream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            fileService.Invoking(service => service.AddFile(stream, "test.txt", "text/plain", 10, null)).ShouldThrow
                <ArgumentNullException>();
        }

        [Fact]
        public void FileService_AddFile_CreatesANewFileRecord()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

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
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), "test-file.txt", null, 0, mediaCategory);

            mediaFile.name.Should().Be("test_file.txt");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentTypeAsSet()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), null, "text/plain", 0, mediaCategory);

            mediaFile.Type.Should().Be("text/plain");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentLengthAsSet()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var mediaFile = fileService.AddFile(GetDefaultStream(), null, "text/plain", 1234, mediaCategory);

            mediaFile.size.Should().Be(1234);
        }

        [Fact]
        public void FileService_AddFile_ShouldSetFileExtension()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            Stream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = Session.Get<MediaFile>(1);
            file.FileExtension.Should().Be(".txt");
        }

        [Fact]
        public void FileService_AddFile_ShouldSetFileLocation()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings { MediaDirectory = "/media" }, fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            Stream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = Session.Get<MediaFile>(1);
            file.FileLocation.Should().Be("/media/test-category/test.txt");
        }

        [Fact]
        public void FileService_AddFile_MediaCategoryShouldHaveFile()
        {
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

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
            var fileSystem = A.Fake<IFileSystem>();

            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));

            Stream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

            var fileService = new FileService(Session, new SiteSettings { MediaDirectory = "/media" }, fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            fileService.AddFile(stream, "test.txt", "text/plain", 10, mediaCategory);

            A.CallTo(() => fileSystem.SaveFile(stream, "/media/test-category/test.txt")).MustHaveHappened();
        }

        [Fact]
        public void FileService_GetFiles_ShouldReturnFilesWhichHaveTheCorrectMediaCategoryId()
        {
            var mediaCategory = GetDefaultMediaCategory();
            Session.Transact(session => session.SaveOrUpdate(mediaCategory));
            var file1 = new MediaFile { MediaCategory = mediaCategory, FileLocation = "/test1.txt" };
            var file2 = new MediaFile { MediaCategory = mediaCategory, FileLocation = "/test2.txt" };
            var file3 = new MediaFile { FileLocation = "/test3.txt" };
            mediaCategory.Files.Add(file1);
            mediaCategory.Files.Add(file2);
            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(file1);
                                     session.SaveOrUpdate(file2);
                                     session.SaveOrUpdate(file3);
                                 });
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(Session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var files = fileService.GetFiles(mediaCategory.Id);

            files.Should().HaveCount(2);

            files.Select(x => x.Id).Should().Contain(file1.Id);
            files.Select(x => x.Id).Should().Contain(file2.Id);
            files.Select(x => x.Id).Should().NotContain(file3.Id);
        }

        [Fact]
        public void FileService_GetFile_ShouldCallSessionGetById()
        {
            var session = A.Fake<ISession>();
            var fileSystem = A.Fake<IFileSystem>();
            var fileService = new FileService(session, new SiteSettings(), fileSystem) { OverrideApplicationPath = "C:\\temp\\" };

            var mediaFile = fileService.GetFile(1);

            A.CallTo(() => session.Get<MediaFile>(1)).MustHaveHappened();
        }
    }
}