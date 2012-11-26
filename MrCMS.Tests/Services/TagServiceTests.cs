using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
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

            var tag1 = new Tag { Name = "tag-1" };
            var tag2 = new Tag { Name = "tag-2" };
            var tag3 = new Tag { Name = "not-the-same" };

            Session.Transact(session =>
                                 {
                                     Session.SaveOrUpdate(tag1);
                                     Session.SaveOrUpdate(tag2);
                                     Session.SaveOrUpdate(tag3);
                                 });

            var tags = tagService.Search("tag", 1);

            tags.Should().HaveCount(2);
            tags.First().label.Should().Be("tag-1");
            tags.Skip(1).First().label.Should().Be("tag-2");
        }

        [Fact]
        public void TagService_GetCategories_ShouldReturnTheTagsOfAParent()
        {
            var fakeSession = A.Fake<ISession>();

            var tagService = new TagService(fakeSession);
            var tag1 = new Tag { Name = "tag-1" };

            Session.Transact(session => Session.SaveOrUpdate(tag1));

            var container = new FakeContainer();
            container.SetTags(new List<Tag> {tag1});
            var containerItem = new FakeContainerItem {Parent = container};

            A.CallTo(() => fakeSession.Get<Document>(1)).Returns(containerItem);

            tagService.GetCategories(1).Should().HaveCount(1);
        }
        
        public class FakeContainerItem : Document, IContainerItem
        {
            public string ContainerUrl { get; private set; }
        }

        public class FakeContainer : Document, IDocumentContainer<FakeContainerItem>
        {
            public void SetTags(IList<Tag> tags)
            {
                Tags = tags;
            }
            public string BodyContent { get; private set; }
            public int PageSize { get; private set; }
            public bool AllowPaging { get; private set; }
            public IEnumerable<FakeContainerItem> ChildItems { get; private set; }
        }
    }
}