using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class TagAdminServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public async Task TagAdminService_Search_ShouldReturnTagsStartingWithTerm()
        {
            var tagService = new TagAdminService(Session);

            var tag1 = new Tag {Name = "tag-1", Site = CurrentSite};
            var tag2 = new Tag {Name = "tag-2", Site = CurrentSite};
            var tag3 = new Tag {Name = "not-the-same", Site = CurrentSite};

            await Session.TransactAsync(async session =>
            {
                await Session.SaveOrUpdateAsync(tag1);
                await Session.SaveOrUpdateAsync(tag2);
                await Session.SaveOrUpdateAsync(tag3);
            });

            IEnumerable<AutoCompleteResult> tags = await tagService.Search("tag");

            tags.Should().HaveCount(2);
            tags.First().label.Should().Be("tag-1");
            tags.Skip(1).First().label.Should().Be("tag-2");
        }

        public class FakeContainer : Document
        {
            public virtual void SetTags(ISet<Tag> tags)
            {
                Tags = tags;
            }
        }

        public class FakeContainerItem : Document
        {
        }
    }
}