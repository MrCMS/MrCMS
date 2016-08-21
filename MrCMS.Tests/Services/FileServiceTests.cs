using System.IO;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class FileServiceTests
    {
        public FileServiceTests()
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
            
            _fileService = new FileService(_fileSystem,
                A.Fake<IImageProcessor>(), _mediaSettings, _currentSite, 
                _mediaFileRepository, _mediaCategoryRepository, _resizedImageRepository);
        }

        private readonly IFileSystem _fileSystem;
        private readonly MediaSettings _mediaSettings;
        private readonly SiteSettings _siteSettings;
        private FileService _fileService;
        private readonly Site _currentSite = new Site {Id = 1};
        private readonly IRepository<MediaFile> _mediaFileRepository = new InMemoryRepository<MediaFile>();
        private readonly IRepository<MediaCategory> _mediaCategoryRepository = new InMemoryRepository<MediaCategory>();
        private readonly IRepository<ResizedImage> _resizedImageRepository = new InMemoryRepository<ResizedImage>();

        private MediaCategory GetDefaultMediaCategory()
        {
            var defaultMediaCategory = new MediaCategory {Name = "test-category", UrlSegment = "test-category"};
            return defaultMediaCategory;
        }

        private static MemoryStream GetDefaultStream()
        {
            return new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
        }

        [Fact]
        public void FileService_AddFile_CallsSaveToFileSystemOfIFileSystem()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            Stream stream = GetDefaultStream();

            _fileService.AddFile(stream, "test.txt", "text/plain", 10, mediaCategory);

            A.CallTo(() => _fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain")).MustHaveHappened();
        }

        [Fact]
        public void FileService_AddFile_CreatesANewFileRecord()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            _fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            _mediaFileRepository.Query().Count().Should().Be(1);
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentLengthAsSet()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            var mediaFile = _fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.ContentLength.Should().Be(1234);
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameContentTypeAsSet()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            var mediaFile = _fileService.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            mediaFile.ContentType.Should().Be("text/plain");
        }

        [Fact]
        public void FileService_AddFile_FileShouldHaveSameNameAsSet()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            var mediaFile = _fileService.AddFile(GetDefaultStream(), "test-file.txt", null, 0, mediaCategory);

            mediaFile.FileName.Should().Be("test-file.txt");
        }

        [Fact]
        public void FileService_AddFile_MediaCategoryShouldHaveFile()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);

            _fileService.AddFile(GetDefaultStream(), "test1.txt", "text/plain", 1234, mediaCategory);

            mediaCategory.Files.Should().HaveCount(1);
        }

        [Fact]
        public void FileService_AddFile_ShouldSetFileExtension()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            Stream stream = GetDefaultStream();

            _fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = _mediaFileRepository.Get(1);
            file.FileExtension.Should().Be(".txt");
        }

        [Fact]
        public void FileService_AddFile_ShouldSetFileLocation()
        {
            var mediaCategory = GetDefaultMediaCategory();
            _mediaCategoryRepository.Add(mediaCategory);
            Stream stream = new MemoryStream(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
            A.CallTo(() => _fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain"))
                .Returns("/content/upload/1/test-category/test.txt");

            _fileService.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            var file = _mediaFileRepository.Get(1);
            file.FileUrl.Should().Be("/content/upload/1/test-category/test.txt");
        }

        [Fact]
        public void FileService_GetFileByLocation_ReturnsAMediaFileIfOneIsSavedWithAMatchingLocation()
        {
            const string fileUrl = "location.jpg";
            var mediaFile = new MediaFile
            {
                FileUrl = fileUrl
            };
            _mediaFileRepository.Add(mediaFile);

            var fileByLocation = _fileService.GetFileByUrl(fileUrl);

            fileByLocation.Should().Be(mediaFile);
        }
    }
}