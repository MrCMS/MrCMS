using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Services;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using MrCMS.Tests.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class DocumentTagsUpdateServiceTests
    {
        public DocumentTagsUpdateServiceTests()
        {
            _tagRepository = new InMemoryRepository<Tag>();
            _webpageRepository = new InMemoryRepository<Document>();
            _documentTagsUpdateService = new DocumentTagsUpdateService(_webpageRepository, _tagRepository);
        }

        private readonly DocumentTagsUpdateService _documentTagsUpdateService;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<Document> _webpageRepository;
        private readonly List<string> _tags = new List<string> { "test" };

        private IEnumerable<Tag> GetAllTags()
        {
            return _tagRepository.Query();
        }

        [Fact]
        public void IfTagIsRemovedFromWebpageShouldNotRemoveTagFromList()
        {
            var tag = new Tag { Name = "Test" };
            _tagRepository.Add(tag);
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };

            _documentTagsUpdateService.SetTags(new List<string>(),
                webpage);

            GetAllTags().Should().HaveCount(1);
        }

        [Fact]
        public void ShouldAddATagToTagListIfItIsNew()
        {
            GetAllTags().Should().HaveCount(0);

            _documentTagsUpdateService.SetTags(_tags,
                new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldAssignExistingTagIfItIsADuplicate()
        {
            _tagRepository.Add(new Tag { Name = "Test" });
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            _documentTagsUpdateService.SetTags(_tags,
                webpage);

            webpage.Tags.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Name.Should().Be("Test");
        }

        [Fact]
        public void ShouldAssignTagToWebpage()
        {
            GetAllTags().Should().HaveCount(0);
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            _documentTagsUpdateService.SetTags(_tags,
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

            _documentTagsUpdateService.SetTags(_tags,
                webpage);

            webpage.Tags.ElementAt(0).Documents.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Documents.ElementAt(0).Should().Be(webpage);
        }

        [Fact]
        public void ShouldNotAddDuplicateTags()
        {
            _tagRepository.Add(new Tag { Name = "test" });
            GetAllTags().Should().HaveCount(1);

            _documentTagsUpdateService.SetTags(_tags,
                new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public void ShouldNotAddDuplicateTagsBasedOnCase()
        {
            _tagRepository.Add(new Tag { Name = "Test" });
            GetAllTags().Should().HaveCount(1);

            _documentTagsUpdateService.SetTags(_tags,
                new BasicMappedWebpage());

            GetAllTags().Should().HaveCount(1);
            GetAllTags().ElementAt(0).Name.Should().Be("Test");
        }

        [Fact]
        public void ShouldRemoveTagIfItIsNoLongerAssignedWebpage()
        {
            var tag = new Tag { Name = "Test" };
            _tagRepository.Add(tag);
            GetAllTags().Should().HaveCount(1);
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };

            _documentTagsUpdateService.SetTags(new List<string>(),
                webpage);

            webpage.Tags.Should().HaveCount(0);
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

            _documentTagsUpdateService.SetTags(new List<string>(), webpage);

            tag.Documents.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfDocumentIsNullThrowArgumentNullException()
        {
            _documentTagsUpdateService.Invoking(service => service.SetTags((string)null, null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfTagsIsNullForANewDocumentTheTagListShouldBeEmpty()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags((string)null, textPage);

            textPage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfTagsHasOneStringTheTagListShouldHave1Tag()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags("test tag", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_IfTagsHasTwoCommaSeparatedTagsTheTagListShouldHave2Tags()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldTrimTagNames()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags("test 1, test 2", textPage);

            textPage.Tags.ElementAt(1).Name.Should().Be("test 2");
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldAddTagsToDocument()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotRecreateTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            _webpageRepository.Add(textPage);
            _tagRepository.Add(tag1);
            _tagRepository.Add(tag2);

            _documentTagsUpdateService.SetTags(textPage.TagList, textPage);

            _tagRepository.Query().Count().Should().Be(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotReaddSetTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            _webpageRepository.Add(textPage);
            _tagRepository.Add(tag1);
            _tagRepository.Add(tag2);

            _documentTagsUpdateService.SetTags(textPage.TagList, textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldRemoveTagsNotIncluded()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            _webpageRepository.Add(textPage);
            _tagRepository.Add(tag1);
            _tagRepository.Add(tag2);

            _documentTagsUpdateService.SetTags("test 1", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldAssignDocumentToTag()
        {
            var textPage = new StubWebpage();
            _webpageRepository.Add(textPage);

            _documentTagsUpdateService.SetTags("test 1", textPage);

            var tags = textPage.Tags;
            tags.Should().HaveCount(1);
            tags.First().Documents.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldRemoveTheDocumentFromTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);
            tag1.Documents.Add(textPage);
            tag2.Documents.Add(textPage);

            _webpageRepository.Add(textPage);
            _tagRepository.Add(tag1);
            _tagRepository.Add(tag2);

            _documentTagsUpdateService.SetTags("test 1", textPage);

            tag1.Documents.Should().HaveCount(1);
            tag2.Documents.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotCreateTagsWithEmptyNames()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags("test 1,,test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentTagsAdminService_SetTags_ShouldNotCreateTagsWithEmptyNamesForTrailingComma()
        {
            var textPage = new StubWebpage();

            _documentTagsUpdateService.SetTags("test 1, test 2, ", textPage);

            textPage.Tags.Should().HaveCount(2);
        }
    }
}