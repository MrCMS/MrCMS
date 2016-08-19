using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using MrCMS.Tests.TestSupport;
using NHibernate.Linq;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateTagsService_SetTagsTests
    {
        private readonly UpdateTagsService _updateTagsService;
        private InMemoryRepository<Tag> _tagRepository;

        public UpdateTagsService_SetTagsTests()
        {
            _tagRepository = new InMemoryRepository<Tag>();
            _updateTagsService = new UpdateTagsService(_tagRepository);
        }

        [Fact]
        public void ShouldAddATagToTagListIfItIsNew()
        {
            GetAllTags().Should().HaveCount(0);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldNotAddDuplicateTags()
        {
            _tagRepository.Add(new Tag {Name = "test"});
            GetAllTags().Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldNotAddDuplicateTagsBasedOnCase()
        {
            _tagRepository.Add(new Tag {Name = "Test"});
            GetAllTags().Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("Test");
        }

        [Fact]
        public void ShouldAssignTagToWebpage()
        {
            GetAllTags().Should().HaveCount(0);
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      webpage);

            webpage.Tags.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldAssignWebpageToTag()
        {
            GetAllTags().Should().HaveCount(0);
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      webpage);

            webpage.Tags.ElementAt(0).Documents.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Documents.ElementAt(0).Should().Be(webpage);
        }

        [Fact]
        public void ShouldAssignExistingTagIfItIsADuplicate()
        {
            _tagRepository.Add(new Tag {Name = "Test"});
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      webpage);

            webpage.Tags.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Name.Should().Be("Test");
        }

        private IEnumerable<Tag> GetAllTags()
        {
            return _tagRepository.Query();
        }

        [Fact]
        public void ShouldRemoveTagIfItIsNoLongerAssignedWebpage()
        {
            var tag = new Tag { Name = "Test" };
            _tagRepository.Add(tag);
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { } },
                                                      webpage);

            webpage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public void IfTagIsRemovedFromWebpageShouldNotRemoveTagFromList()
        {
            var tag = new Tag { Name = "Test" };
            _tagRepository.Add(tag);
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { } },
                                                      webpage);

            GetAllTags().Should().HaveCount(1);
        }

        [Fact]
        public void ShouldRemoveTheWebpageFromTheTagsWebpages()
        {
            var tag = new Tag { Name = "Test" };
            _tagRepository.Add(tag);
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };
            tag.Documents.Add(webpage);
            tag.Documents.Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string>() }, webpage);

            tag.Documents.Should().HaveCount(0);
        }
    }
}