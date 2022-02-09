using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using Xunit;
using MrCMS.Helpers;
using System.Threading.Tasks;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Services
{
    public class FileServiceTests : InMemoryDatabaseTest
    {
        private IFileSystem _fileSystem;
        private MediaSettings _mediaSettings;
        private SiteSettings _siteSettings;
        private IFileSystemFactory _fileSystemFactory;
        private ICurrentSiteLocator _siteLocator;

        private FileService GetFileService(ISession session = null, IFileSystem fileSystem = null)
        {
            _fileSystemFactory = A.Fake<IFileSystemFactory>();
            _fileSystem = fileSystem ?? A.Fake<IFileSystem>();
            A.CallTo(() => _fileSystemFactory.GetForCurrentSite()).Returns(_fileSystem);
            
            _siteLocator = A.Fake<ICurrentSiteLocator>();
            A.CallTo(() => _siteLocator.GetCurrentSite()).Returns(CurrentSite);

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
            return new FileService(session ?? Session, _fileSystemFactory,
                A.Fake<IImageProcessor>(), _mediaSettings, _siteLocator , _siteSettings);
        }

        [Fact]
        public async Task FileService_AddFile_CreatesANewFileRecord()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));
            await fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            (await Session.QueryOver<MediaFile>().RowCountAsync()).Should().Be(1);
        }

        private static MediaCategory GetDefaultMediaCategory()
        {
            return new MediaCategory {Name = "test-category", Path = "test-category"};
        }

        [Fact]
        public async Task FileService_AddFile_FileShouldHaveSameNameAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));
            var mediaFile = await fileService.AddFile(GetDefaultStream(), "test-file.txt", null, 0, mediaCategory);

            mediaFile.FileName.Should().Be("test-file.txt");
        }

        [Fact]
        public async Task FileService_AddFile_FileShouldHaveSameContentTypeAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));
            var mediaFile = await fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            mediaFile.ContentType.Should().Be("text/plain");
        }

        [Fact]
        public async Task FileService_AddFile_FileShouldHaveSameContentLengthAsSet()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));
            var mediaFile =
                await fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.ContentLength.Should().Be(1234);
        }

        [Fact]
        public async Task FileService_AddFile_ShouldSetFileExtension()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));
            Stream stream = GetDefaultStream();

            await fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = Session.Get<MediaFile>(1);
            file.FileExtension.Should().Be(".txt");
        }

        [Fact]
        public async Task FileService_AddFile_ShouldSetFileLocation()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));
            Stream stream = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
            A.CallTo(() => _fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain"))
                .Returns("/content/upload/1/test-category/test.txt");

            await fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = Session.Get<MediaFile>(1);
            file.FileUrl.Should().Be("/content/upload/1/test-category/test.txt");
        }

        [Fact]
        public async Task FileService_AddFile_MediaCategoryShouldHaveFile()
        {
            var fileService = GetFileService();

            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));

            await fileService.AddFile(GetDefaultStream(), "test1.txt", "text/plain", 1234, mediaCategory);

            mediaCategory.Files.Should().HaveCount(1);
        }

        private static MemoryStream GetDefaultStream()
        {
            return new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
        }

        [Fact]
        public async Task FileService_AddFile_CallsSaveToFileSystemOfIFileSystem()
        {
            var mediaCategory = GetDefaultMediaCategory();
            await Session.TransactAsync(session => session.SaveOrUpdateAsync(mediaCategory));

            Stream stream = GetDefaultStream();

            var fileService = GetFileService();

            await fileService.AddFile(stream, "test.txt", "text/plain", 10, mediaCategory);

            A.CallTo(() => _fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain")).MustHaveHappened();
        }

        [Fact]
        public async Task FileService_GetFileByLocation_ReturnsAMediaFileIfOneIsSavedWithAMatchingLocation()
        {
            var fileService = GetFileService();

            const string fileUrl = "location.jpg";
            var mediaFile = new MediaFile
            {
                FileUrl = fileUrl
            };
            await Session.TransactAsync(session => session.SaveAsync(mediaFile));

            var fileByLocation = fileService.GetFileByUrl(fileUrl);

            fileByLocation.Should().Be(mediaFile);
        }
    }
}