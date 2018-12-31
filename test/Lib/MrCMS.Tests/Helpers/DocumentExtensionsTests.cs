using FluentAssertions;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Helpers
{
    public class DocumentExtensionsTests
    {
        [Fact]
        public void DocumentExtensions_GetVersion_ReturnsNullIfThereIsNoMatchingVersionInCollection()
        {
            var doc = new StubDocument();

            doc.GetVersion(1).Should().Be(null);
        }
    }
}