using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using Xunit;

namespace MrCMS.Tests.Entities
{
    public class MediaFileTests
    {
        [Fact]
        public void MediaFile_IsImage_ReturnsTrueForAFileWithExtensionJpg()
        {
            var mediaFile = new MediaFile {FileExtension = ".jpg"};

            mediaFile.IsImage().Should().BeTrue();
        }

        [Fact]
        public void MediaFile_IsImage_ReturnsTrueForAFileWithExtensionJpeg()
        {
            var mediaFile = new MediaFile {FileExtension = ".jpeg"};

            mediaFile.IsImage().Should().BeTrue();
        }

        [Fact]
        public void MediaFile_IsImage_ReturnsTrueForAFileWithExtensionGif()
        {
            var mediaFile = new MediaFile {FileExtension = ".gif"};

            mediaFile.IsImage().Should().BeTrue();
        }

        [Fact]
        public void MediaFile_IsImage_ReturnsTrueForAFileWithExtensionPng()
        {
            var mediaFile = new MediaFile {FileExtension = ".png"};

            mediaFile.IsImage().Should().BeTrue();
        }

        [Fact]
        public void MediaFile_IsImage_ReturnsFalseForAFileWithExtensionTxt()
        {
            var mediaFile = new MediaFile {FileExtension = ".txt"};

            mediaFile.IsImage().Should().BeFalse();
        }

        [Fact]
        public void MediaFile_IsImage_ReturnsTrueForAFileWithExtensionJPGAllCaps()
        {
            var mediaFile = new MediaFile {FileExtension = ".JPG"};

            mediaFile.IsImage().Should().BeTrue();
        }

        [Fact]
        public void MediaFile_IsImage_ReturnsTrueForAFileWithExtensionJpgMixedCasing()
        {
            var mediaFile = new MediaFile {FileExtension = ".JpG"};

            mediaFile.IsImage().Should().BeTrue();
        }
    }
}