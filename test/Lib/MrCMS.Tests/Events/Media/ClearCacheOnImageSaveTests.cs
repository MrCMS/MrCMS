using FluentAssertions;
using MrCMS.Events.Media;
using Xunit;

namespace MrCMS.Tests.Events.Media
{
    public class ClearCacheOnImageSaveTests
    {
        [Fact]
        public void LogicGetsTheFileUrlWithoutExtensionForLocalFileSystem()
        {
            var url = "/content/upload/1/default/test-image.png";

            var result = GetClearCacheOnImageSave().GetUrlWithoutExtension(url);

            result.Should().Be("/content/upload/1/default/test-image");
        }
        [Fact]
        public void LogicGetsTheFileUrlWithoutExtensionForAzureFileSystem()
        {
            var url = "https://test.blob.core.windows.net/test/1/default/test-image.png";

            var result = GetClearCacheOnImageSave().GetUrlWithoutExtension(url);

            result.Should().Be("https://test.blob.core.windows.net/test/1/default/test-image");
        }

        private static ClearCacheOnImageSave GetClearCacheOnImageSave()
        {
            return new ClearCacheOnImageSave(null);
        }
    }
}