using FluentAssertions;
using MrCMS.Helpers;
using Xunit;

namespace MrCMS.Tests.Helpers
{
    public class UrlHelperExtenstionTests
    {
        [Fact]
        public void UrlAppendsForwardSlashWhenDoesntExist()
        {
            const string url = "some-url";

            url.GetRelativeUrl().Should().Be("/some-url");
        }
        
        [Fact]
        public void Url_Should_Return_Same_When_Exists_ForwardSlash()
        {
            const string url = "/some-url";

            url.GetRelativeUrl().Should().Be("/some-url");
        }
        
        [Fact]
        public void Url_Empty_Should_Return_Empty()
        {
            const string url = "";

            url.GetRelativeUrl().Should().Be("");
        }
        
        [Fact]
        public void Multiple_Forward_Slashes_Should_Be_Fixed()
        {
            const string url = "//some-url";

            url.GetRelativeUrl().Should().Be("/some-url");
        }
    }
}