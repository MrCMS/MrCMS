using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Settings;

using Xunit;
using MrCMS.Helpers;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using MockQueryable.FakeItEasy;
using MrCMS.Data;
using MrCMS.TestSupport;

namespace MrCMS.Tests.Services
{
    public class FileServiceTests : MrCMSTest
    {
        //private IFileSystem _fileSystem;
        //private MediaSettings _mediaSettings;
        //private SiteSettings _siteSettings;


        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_CreatesANewFileRecord([Frozen] IRepository<MediaFile> mediaFileRepository, FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();
            var mediaFile = await sut.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            A.CallTo(() => mediaFileRepository.Add(mediaFile, default)).MustHaveHappened();
            //Context.QueryOver<MediaFile>().RowCount().Should().Be(1);
        }

        private static MediaCategory GetDefaultMediaCategory()
        {
            return new MediaCategory { Name = "test-category", UrlSegment = "test-category" };
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_FileShouldHaveSameNameAsSet(FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();
            var mediaFile = await sut.AddFile(GetDefaultStream(), "test-file.txt", null, 0, mediaCategory);

            mediaFile.FileName.Should().Be("test-file.txt");
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_FileShouldHaveSameContentTypeAsSet(FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();
            var mediaFile = await sut.AddFile(GetDefaultStream(), "test.txt", "text/plain", 0, mediaCategory);

            mediaFile.ContentType.Should().Be("text/plain");
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_FileShouldHaveSameContentLengthAsSet(FileService sut)
        {

            var mediaCategory = GetDefaultMediaCategory();
            var mediaFile = await sut.AddFile(GetDefaultStream(), "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.ContentLength.Should().Be(1234);
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_ShouldSetFileExtension(FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();
            Stream stream = GetDefaultStream();

            var mediaFile = await sut.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.FileExtension.Should().Be(".txt");
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_ShouldSetFileLocation([Frozen] IFileSystem fileSystem, FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();
            Stream stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            A.CallTo(() => fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain"))
             .Returns("/content/upload/1/test-category/test.txt");

            var mediaFile = await sut.AddFile(stream, "test.txt", "text/plain", 1234, mediaCategory);

            mediaFile.FileUrl.Should().Be("/content/upload/1/test-category/test.txt");
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_MediaCategoryShouldHaveFile(FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();

            await sut.AddFile(GetDefaultStream(), "test1.txt", "text/plain", 1234, mediaCategory);

            mediaCategory.Files.Should().HaveCount(1);
        }

        private static MemoryStream GetDefaultStream()
        {
            return new MemoryStream(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_AddFile_CallsSaveToFileSystemOfIFileSystem([Frozen] IFileSystem fileSystem, FileService sut)
        {
            var mediaCategory = GetDefaultMediaCategory();

            Stream stream = GetDefaultStream();

            await sut.AddFile(stream, "test.txt", "text/plain", 10, mediaCategory);

            A.CallTo(() => fileSystem.SaveFile(stream, "1/test-category/test.txt", "text/plain")).MustHaveHappened();
        }

        [Theory, AutoFakeItEasyData]
        public async Task FileService_GetFileByLocation_ReturnsAMediaFileIfOneIsSavedWithAMatchingLocation([Frozen] IRepository<MediaFile> repository, FileService sut)
        {
            const string fileUrl = "location.jpg";
            var mediaFile = new MediaFile
            {
                FileUrl = fileUrl
            };
            
            A.CallTo(() => repository.Query()).ReturnsAsAsyncQueryable(mediaFile);

            var fileByLocation = await sut.GetFileByUrl(fileUrl);

            fileByLocation.Should().Be(mediaFile);
        }
    }
}