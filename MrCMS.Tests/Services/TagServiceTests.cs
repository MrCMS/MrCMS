using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Services;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class TagServiceTests :InMemoryDatabaseTest
    {
        [Fact]
        public void TagService_Search_ShouldReturnTagsStartingWithTerm()
        {
            var tagService = new TagService(Session);

                                     var tag1 = new Tag {Name = "tag-1"};
                                     var tag2 = new Tag {Name = "tag-2"};
                                     var tag3 = new Tag {Name = "not-the-same"};
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
    }
}