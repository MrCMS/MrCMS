using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using NHibernate.Linq;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateTagsService_SetTagsTests : InMemoryDatabaseTest
    {
        private readonly UpdateTagsService _updateTagsService;

        public UpdateTagsService_SetTagsTests()
        {
            _updateTagsService = new UpdateTagsService(Session);
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
            Session.Transact(session => session.Save(new Tag { Name = "test" }));
            GetAllTags().Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldNotAddDuplicateTagsBasedOnCase()
        {
            Session.Transact(session => session.Save(new Tag { Name = "Test" }));
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
            Session.Transact(session => session.Save(new Tag { Name = "Test" }));
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
            return Session.Query<Tag>();
        }

        [Fact]
        public void ShouldRemoveTagIfItIsNoLongerAssignedWebpage()
        {
            var tag = new Tag { Name = "Test" };
            Session.Transact(session => session.Save(tag));
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashedSet<Tag> { tag } };

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { } },
                                                      webpage);

            webpage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public void IfTagIsRemovedFromWebpageShouldNotRemoveTagFromList()
        {
            var tag = new Tag { Name = "Test" };
            Session.Transact(session => session.Save(tag));
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashedSet<Tag> { tag } };

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { } },
                                                      webpage);

            GetAllTags().Should().HaveCount(1);
        }

        [Fact]
        public void ShouldRemoveTheWebpageFromTheTagsWebpages()
        {
            var tag = new Tag { Name = "Test" };
            Session.Transact(session => session.Save(tag));
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashedSet<Tag> { tag } };
            tag.Documents.Add(webpage);
            tag.Documents.Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string>() }, webpage);

            tag.Documents.Should().HaveCount(0);
        }
    }
}