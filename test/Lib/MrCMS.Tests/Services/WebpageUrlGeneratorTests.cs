using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class WebpageUrlGeneratorTests : MrCMSTest
    {
        private readonly IUrlValidationService _urlValidationService;
        private readonly WebpageUrlService _webpageUrlService;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IRepository<PageTemplate> _pageTemplateRepository;
        private readonly IConfigurationProvider _configurationProvider;

        public WebpageUrlGeneratorTests()
        {
            _urlValidationService = A.Fake<IUrlValidationService>();
            _webpageRepository = A.Fake<IRepository<Webpage>>();
            _pageTemplateRepository = A.Fake<IRepository<PageTemplate>>();
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(A<string>.Ignored, A<int?>.Ignored)).Returns(true);
            _configurationProvider = A.Fake<IConfigurationProvider>();
            _webpageUrlService = new WebpageUrlService(_webpageRepository, _pageTemplateRepository, _urlValidationService, ServiceProvider, _configurationProvider);
        }

        [Fact]
        public async Task WebpageUrlGenerator_GetDocumentUrl_ReturnsAUrlBasedOnTheHierarchyIfTheFlagIsSetToTrue()
        {
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page" };

            A.CallTo(() => _webpageRepository.Readonly()).ReturnsAsAsyncQueryable(textPage);

            string documentUrl = await _webpageUrlService.Suggest(new SuggestParams
            {
                PageName = "Nested Page",
                ParentId = textPage.Id,
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = true
            });

            documentUrl.Should().Be("test-page/nested-page");
        }

        [Fact]
        public async Task WebpageUrlGenerator_GetDocumentUrl_ReturnsAUrlBasedOnTheNameIfTheFlagIsSetToFalse()
        {
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page" };

            A.CallTo(() => _webpageRepository.Readonly()).ReturnsAsAsyncQueryable(textPage);

            string documentUrl = await _webpageUrlService.Suggest(new SuggestParams
            {
                PageName = "Nested Page",
                ParentId = textPage.Id,
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = false
            });

            documentUrl.Should().Be("nested-page");
        }

        [Fact]
        public async Task WebpageUrlGenerator_GetDocumentUrlWithExistingName_ShouldReturnTheUrlWithADigitAppended()
        {
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Id = 123 };
            var textPage = new BasicMappedWebpage
            {
                Name = "Test Page",
                Parent = parent,
                UrlSegment = "parent/test-page",
                Id = 234
            };
            A.CallTo(() => _webpageRepository.Readonly()).ReturnsAsAsyncQueryable(parent, textPage);
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page", null))
                .Returns(false);

            string documentUrl = await _webpageUrlService.Suggest(new SuggestParams
            {
                PageName = "Nested Page",
                ParentId = textPage.Id,
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = true
            });

            documentUrl.Should().Be("parent/test-page/nested-page-1");
        }

        [Fact]
        public async Task
            WebpageUrlGenerator_GetDocumentUrlWithExistingName_MultipleFilesWithSameNameShouldNotAppendMultipleDigits()
        {
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Id = 123 };
            var textPage = new BasicMappedWebpage
            {
                Name = "Test Page",
                Parent = parent,
                UrlSegment = "parent/test-page",
                Id = 234
            };
            A.CallTo(() => _webpageRepository.Readonly()).ReturnsAsAsyncQueryable(parent, textPage);
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page", null))
                .Returns(false);
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page-1", null))
                .Returns(false);

            string documentUrl = await _webpageUrlService.Suggest(new SuggestParams
            {
                PageName = "Nested Page",
                ParentId = textPage.Id,
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = true
            });

            documentUrl.Should().Be("parent/test-page/nested-page-2");
        }
    }
}