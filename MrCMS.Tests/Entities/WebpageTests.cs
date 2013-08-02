using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Tests.Entities
{
    public class WebpageTests : InMemoryDatabaseTest
    {
        [Fact]
        public void Webpage_PublishedChildren_DoesNotReturnNonWebpages()
        {
            var doc = new StubWebpage();

            doc.SetChildren(new List<Webpage> { new StubWebpage() });

            doc.PublishedChildren.Should().HaveCount(0);
        }

        [Fact]
        public void Webpage_PublishedChildren_PublishedWebpageIsReturned()
        {
            var doc = new StubWebpage();

            var document = new BasicMappedWebpage { PublishOn = CurrentRequestData.Now.AddDays(-1) };
            doc.SetChildren(new List<Webpage> { document });

            doc.PublishedChildren.Should().Contain(document);
        }

        [Fact]
        public void Webpage_PublishedChildren_UnpublishedWebpageIsNotReturned()
        {
            var doc = new StubWebpage();

            var document = new BasicMappedWebpage { PublishOn = null };
            doc.SetChildren(new List<Webpage> { document });

            doc.PublishedChildren.Should().NotContain(document);
        }
    }
}