using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using NHibernate;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class TagServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void TagService_Search_ShouldReturnTagsStartingWithTerm()
        {
            var tagService = new TagService(Session);

            var tag1 = new Tag { Name = "tag-1", Site = CurrentSite };
            var tag2 = new Tag { Name = "tag-2", Site = CurrentSite };
            var tag3 = new Tag { Name = "not-the-same", Site = CurrentSite };

            Session.Transact(session =>
                                 {
                                     Session.SaveOrUpdate(tag1);
                                     Session.SaveOrUpdate(tag2);
                                     Session.SaveOrUpdate(tag3);
                                 });

            Document document = new StubDocument { Site = CurrentSite };
            var tags = tagService.Search(document, "tag");

            tags.Should().HaveCount(2);
            tags.First().label.Should().Be("tag-1");
            tags.Skip(1).First().label.Should().Be("tag-2");
        }

        [Fact]
        public void TagService_GetTags_ShouldReturnTheTagsOfAParent()
        {
            var fakeSession = A.Fake<ISession>();

            var tagService = new TagService(fakeSession);
            var tag1 = new Tag { Name = "tag-1", Site = CurrentSite };

            Session.Transact(session => Session.SaveOrUpdate(tag1));

            var container = new FakeContainer { Site = CurrentSite };
            container.SetTags(new HashedSet<Tag> { tag1 });
            var containerItem = new FakeContainerItem { Parent = container, Site = CurrentSite };

            tagService.GetTags(containerItem).Should().HaveCount(1);
        }

        public class FakeContainerItem : Document
        {
        }

        public class FakeContainer : Document
        {
            public virtual void SetTags(Iesi.Collections.Generic.ISet<Tag> tags)
            {
                Tags = tags;
            }
        }
    }
}