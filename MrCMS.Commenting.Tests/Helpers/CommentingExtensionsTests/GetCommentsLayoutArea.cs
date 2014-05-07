using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Commenting;
using Xunit;
using MrCMS.Web.Apps.Commenting.Extensions;

namespace MrCMS.Commenting.Tests.Helpers.CommentingExtensionsTests
{
    public class GetCommentsLayoutArea : MrCMSTest
    {
        [Fact]
        public void PageWithNullLayoutReturnsNull()
        {
            var basicMappedWebpage = new BasicMappedWebpage();

            basicMappedWebpage.GetCommentsLayoutArea().Should().BeNull();
        }

        [Fact]
        public void PageWithLayoutButNoMatchingAreaReturnsNull()
        {
            var basicMappedWebpage = new BasicMappedWebpage { Layout = new Layout() };

            basicMappedWebpage.GetCommentsLayoutArea().Should().BeNull();
        }

        [Fact]
        public void PageWithLayoutAreaWithValidNameShouldReturnThatArea()
        {
            var layoutArea = new LayoutArea { AreaName = CommentingApp.LayoutAreaName };
            var basicMappedWebpage = new BasicMappedWebpage { Layout = new Layout { LayoutAreas = new List<LayoutArea> { layoutArea } } };

            basicMappedWebpage.GetCommentsLayoutArea().Should().Be(layoutArea);
        }
    }
}