using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.TestSupport;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class TagAdminServiceTests 
    {
        private InMemoryRepository<Tag> _inMemoryRepository;

        [Fact]
        public void TagAdminService_Search_ShouldReturnTagsStartingWithTerm()
        {
            _inMemoryRepository = new InMemoryRepository<Tag>();
            var tagService = new TagAdminService(_inMemoryRepository);

            var tag1 = new Tag {Name = "tag-1", };
            var tag2 = new Tag {Name = "tag-2", };
            var tag3 = new Tag {Name = "not-the-same", };

            _inMemoryRepository.Add(tag1);
            _inMemoryRepository.Add(tag2);
            _inMemoryRepository.Add(tag3);

            IEnumerable<AutoCompleteResult> tags = tagService.Search("tag");

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