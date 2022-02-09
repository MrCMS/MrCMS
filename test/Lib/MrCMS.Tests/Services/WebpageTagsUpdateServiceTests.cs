using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class WebpageTagsUpdateServiceTests
    {
        public WebpageTagsUpdateServiceTests()
        {
            _tagRepository = A.Fake<IRepository<Tag>>();
            _webpageRepository = A.Fake<IRepository<Webpage>>();
            _getExistingTag = A.Fake<IGetExistingTag>();
            _sut = new WebpageTagsUpdateService(_webpageRepository, _tagRepository, _getExistingTag);
        }

        private readonly IRepository<Tag> _tagRepository;
        private readonly IWebpageTagsUpdateService _sut;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly List<string> _tags = new() { "test" };
        private readonly IGetExistingTag _getExistingTag;

        [Fact]
        public async Task ShouldAssignExistingTagIfItIsADuplicate()
        {
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);
            A.CallTo(() => _getExistingTag.GetTag("test")).Returns((Tag)null);

            await _sut.SetTags(_tags,
                webpage);

            webpage.Tags.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public async Task ShouldAssignTagToWebpage()
        {
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);
            A.CallTo(() => _getExistingTag.GetTag("test")).Returns((Tag)null);

            await _sut.SetTags(_tags,
                webpage);

            webpage.Tags.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Name.Should().Be("test");
        }

        [Fact]
        public async Task ShouldAssignWebpageToTag()
        {
            var webpage = new BasicMappedWebpage();
            webpage.Tags.Should().HaveCount(0);

            await _sut.SetTags(_tags,
                webpage);

            webpage.Tags.ElementAt(0).Webpages.Should().HaveCount(1);
            webpage.Tags.ElementAt(0).Webpages.ElementAt(0).Should().Be(webpage);
        }

        [Fact]
        public async Task ShouldRemoveTagIfItIsNoLongerAssignedWebpage()
        {
            var tag = new Tag { Name = "Test" };
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };

            await _sut.SetTags(new List<string>(),
                webpage);

            webpage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public async Task ShouldRemoveTheWebpageFromTheTagsWebpages()
        {
            var tag = new Tag { Name = "Test" };
            await _tagRepository.Add(tag);
            var webpage = new BasicMappedWebpage { Tags = new HashSet<Tag> { tag } };
            tag.Webpages.Add(webpage);
            tag.Webpages.Should().HaveCount(1);

            await _sut.SetTags(new List<string>(), webpage);

            tag.Webpages.Should().HaveCount(0);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_IfDocumentIsNullThrowArgumentNullException()
        {
            await _sut.Invoking(service => service.SetTags((string)null, null)).Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_IfTagsIsNullForANewDocumentTheTagListShouldBeEmpty()
        {
            var textPage = new StubWebpage();

            await _sut.SetTags((string)null, textPage);

            textPage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_IfTagsHasOneStringTheTagListShouldHave1Tag()
        {
            var textPage = new StubWebpage();

            await _sut.SetTags("test tag", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_IfTagsHasTwoCommaSeparatedTagsTheTagListShouldHave2Tags()
        {
            var textPage = new StubWebpage();

            await _sut.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldTrimTagNamesOnNewTags()
        {
            var textPage = new StubWebpage();
            A.CallTo(() => _getExistingTag.GetTag(A<string>.Ignored)).Returns((Tag)null);

            await _sut.SetTags("test 1, test 2", textPage);

            textPage.Tags.ElementAt(1).Name.Should().Be("test 2");
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldAddTagsToDocument()
        {
            var textPage = new StubWebpage();

            await _sut.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldNotRecreateTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            A.CallTo(() => _getExistingTag.GetTag("test 1")).Returns(tag1);
            A.CallTo(() => _getExistingTag.GetTag("test 2")).Returns(tag2);

            await _sut.SetTags(new List<string> { "test 1", "test 2" }, textPage);

            textPage.Tags.Should().OnlyContain(x => x == tag1 || x == tag2);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldNotReaddSetTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            await _webpageRepository.Add(textPage);
            await _tagRepository.Add(tag1);
            await _tagRepository.Add(tag2);

            await _sut.SetTags(textPage.TagList, textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldRemoveTagsNotIncluded()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            await _webpageRepository.Add(textPage);
            await _tagRepository.Add(tag1);
            await _tagRepository.Add(tag2);

            await _sut.SetTags("test 1", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldAssignDocumentToTag()
        {
            var textPage = new StubWebpage();
            await _webpageRepository.Add(textPage);

            await _sut.SetTags("test 1", textPage);

            var tags = textPage.Tags;
            tags.Should().HaveCount(1);
            tags.First().Webpages.Should().HaveCount(1);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldRemoveTheDocumentFromTags()
        {
            var textPage = new StubWebpage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);
            tag1.Webpages.Add(textPage);
            tag2.Webpages.Add(textPage);

            await _webpageRepository.Add(textPage);
            await _tagRepository.Add(tag1);
            await _tagRepository.Add(tag2);

            await _sut.SetTags("test 1", textPage);

            tag1.Webpages.Should().HaveCount(1);
            tag2.Webpages.Should().HaveCount(0);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldNotCreateTagsWithEmptyNames()
        {
            var textPage = new StubWebpage();

            await _sut.SetTags("test 1,,test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task WebpageTagsAdminService_SetTags_ShouldNotCreateTagsWithEmptyNamesForTrailingComma()
        {
            var textPage = new StubWebpage();

            await _sut.SetTags("test 1, test 2, ", textPage);

            textPage.Tags.Should().HaveCount(2);
        }
    }
}