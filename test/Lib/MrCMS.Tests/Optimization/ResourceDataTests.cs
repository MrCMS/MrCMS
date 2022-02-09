using FluentAssertions;
using MrCMS.Website.Optimization;
using Xunit;

namespace MrCMS.Tests.Optimization
{
    public class ResourceDataTests
    {
        [Fact]
        public void ResourceData_Get_AssignsIsRemoteToValue()
        {
            var resourceData = ResourceData.Get(true, null);

            resourceData.IsRemote.Should().BeTrue();
        }

        [Fact]
        public void ResourceData_Get_AssignsUrlToValue()
        {
            var resourceData = ResourceData.Get(true, "url");

            resourceData.Url.Should().Be("url");
        }
    }
}