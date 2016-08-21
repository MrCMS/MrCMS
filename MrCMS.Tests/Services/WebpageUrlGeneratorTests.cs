using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.Tests.TestSupport;
using Ninject;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class WebpageUrlGeneratorTests
    {
        private readonly IUrlValidationService _urlValidationService;
        private readonly WebpageUrlService _webpageUrlService;
        private readonly PageDefaultsSettings _pageDefaultsSettings;
        private readonly IRepository<PageTemplate> _pageTemplateRepository = new InMemoryRepository<PageTemplate>();
        private IKernel _kernel = new MockingKernel();
        private Site _currentSite = new Site {Id = 1};

        public WebpageUrlGeneratorTests()
        {
            _urlValidationService = A.Fake<IUrlValidationService>();
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(A<string>.Ignored, A<int?>.Ignored)).Returns(true);
            _pageDefaultsSettings = new PageDefaultsSettings();
            _webpageUrlService = new WebpageUrlService(_pageTemplateRepository, _urlValidationService, _pageDefaultsSettings, _kernel);
        }

        [Fact]
        public void WebpageUrlGenerator_GetDocumentUrl_ReturnsAUrlBasedOnTheHierarchyIfTheFlagIsSetToTrue()
        {
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page", Site = _currentSite };

            string documentUrl = _webpageUrlService.Suggest(textPage, new SuggestParams
            {
                PageName = "Nested Page",
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = true
            });

            documentUrl.Should().Be("test-page/nested-page");
        }

        [Fact]
        public void WebpageUrlGenerator_GetDocumentUrl_ReturnsAUrlBasedOnTheNameIfTheFlagIsSetToFalse()
        {
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page", Site = _currentSite };

            string documentUrl = _webpageUrlService.Suggest(textPage, new SuggestParams
            {
                PageName = "Nested Page",
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = false
            });

            documentUrl.Should().Be("nested-page");
        }

        [Fact]
        public void WebpageUrlGenerator_GetDocumentUrlWithExistingName_ShouldReturnTheUrlWithADigitAppended()
        {
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Site = _currentSite };
            var textPage = new BasicMappedWebpage
            {
                Name = "Test Page",
                Parent = parent,
                UrlSegment = "parent/test-page",
                Site = _currentSite
            };
            //Session.Transact(session =>
            //{
            //    session.SaveOrUpdate(parent);
            //    session.SaveOrUpdate(textPage);
            //});
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page", null))
                .Returns(false);

            string documentUrl = _webpageUrlService.Suggest(textPage, new SuggestParams
            {
                PageName = "Nested Page",
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = true
            });

            documentUrl.Should().Be("parent/test-page/nested-page-1");
        }

        [Fact]
        public void
            WebpageUrlGenerator_GetDocumentUrlWithExistingName_MultipleFilesWithSameNameShouldNotAppendMultipleDigits()
        {
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Site = _currentSite };
            var textPage = new BasicMappedWebpage
            {
                Name = "Test Page",
                Parent = parent,
                UrlSegment = "parent/test-page",
                Site = _currentSite
            };
            //Session.Transact(session =>
            //{
            //    session.SaveOrUpdate(parent);
            //    session.SaveOrUpdate(textPage);
            //});
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page", null))
                .Returns(false);
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page-1", null))
                .Returns(false);

            string documentUrl = _webpageUrlService.Suggest(textPage, new SuggestParams
            {
                PageName = "Nested Page",
                DocumentType = typeof(BasicMappedWebpage).FullName,
                UseHierarchy = true
            });

            documentUrl.Should().Be("parent/test-page/nested-page-2");
        }
    }
}