using FluentAssertions;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Helpers
{
    public class WebpageVersionExtensionsTests
    {
        [Fact]
        public void WebpageVersionExtensions_GetVersion_ReturnsNullIfThereIsNoMatchingVersionInCollection()
        {
            var doc = new StubWebpage();

            doc.GetVersion(1).Should().Be(null);
        }
    }
}