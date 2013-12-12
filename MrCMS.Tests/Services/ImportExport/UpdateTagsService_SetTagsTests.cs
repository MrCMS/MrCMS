using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Iesi.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateTagsService_SetTagsTests : InMemoryDatabaseTest
    {
        private readonly UpdateTagsService _updateTagsService;

        public UpdateTagsService_SetTagsTests()
        {
            _updateTagsService = new UpdateTagsService(Session, CurrentSite);
            _updateTagsService.Inititalise();
        }

        [Fact]
        public void ShouldAddATagToTagListIfItIsNew()
        {
            _updateTagsService.Tags.Should().HaveCount(0);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            _updateTagsService.Tags.Should().HaveCount(1);
            _updateTagsService.Tags.ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldNotAddDuplicateTags()
        {
            Session.Transact(session => session.Save(new Tag { Name = "test" }));
            _updateTagsService.Inititalise();
            _updateTagsService.Tags.Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            _updateTagsService.Tags.Should().HaveCount(1);
            _updateTagsService.Tags.ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldNotAddDuplicateTagsBasedOnCase()
        {
            Session.Transact(session => session.Save(new Tag { Name = "Test" }));
            _updateTagsService.Inititalise();
            _updateTagsService.Tags.Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      new BasicMappedWebpage());

            _updateTagsService.Tags.Should().HaveCount(1);
            _updateTagsService.Tags.ElementAt(0).Name.Should().Be("Test");
        }

        [Fact]
        public void ShouldAssignTagToWebpage()
        {
            _updateTagsService.Tags.Should().HaveCount(0);
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
            _updateTagsService.Tags.Should().HaveCount(0);
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
            _updateTagsService.Inititalise();
            _updateTagsService.Tags.Should().HaveCount(1);
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { "test" } },
                                                      webpage);

            webpage.Tags.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Name.Should().Be("Test");
        }

        [Fact]
        public void ShouldRemoveTagIfItIsNoLongerAssignedWebpage()
        {
            var tag = new Tag { Name = "Test" };
            Session.Transact(session => session.Save(tag));
            _updateTagsService.Inititalise();
            _updateTagsService.Tags.Should().HaveCount(1);
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
            _updateTagsService.Inititalise();
            _updateTagsService.Tags.Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashedSet<Tag> { tag } };

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string> { } },
                                                      webpage);

            _updateTagsService.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldRemoveTheWebpageFromTheTagsWebpages()
        {
            var tag = new Tag { Name = "Test" };
            Session.Transact(session => session.Save(tag));
            _updateTagsService.Inititalise();
            _updateTagsService.Tags.Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashedSet<Tag> { tag } };
            tag.Documents.Add(webpage);
            tag.Documents.Should().HaveCount(1);

            _updateTagsService.SetTags(new DocumentImportDTO { Tags = new List<string>() }, webpage);

            tag.Documents.Should().HaveCount(0);
        }
    }
}