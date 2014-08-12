using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class WebpageUrlGeneratorTests : InMemoryDatabaseTest
    {
        private readonly IUrlValidationService _urlValidationService;
        private readonly WebpageUrlService _webpageUrlService;
        private readonly PageDefaultsSettings _pageDefaultsSettings;

        public WebpageUrlGeneratorTests()
        {
            _urlValidationService = A.Fake<IUrlValidationService>();
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage(A<string>.Ignored, A<int?>.Ignored)).Returns(true);
            _pageDefaultsSettings = new PageDefaultsSettings();
            _webpageUrlService = new WebpageUrlService(_urlValidationService, Session, Kernel, _pageDefaultsSettings);
        }

        [Fact]
        public void WebpageUrlGenerator_GetDocumentUrl_ReturnsAUrlBasedOnTheHierarchyIfTheFlagIsSetToTrue()
        {
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page", Site = CurrentSite };

            Session.Transact(session => session.SaveOrUpdate(textPage));

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
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page", Site = CurrentSite };

            Session.Transact(session => session.SaveOrUpdate(textPage));

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
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Site = CurrentSite };
            var textPage = new BasicMappedWebpage
            {
                Name = "Test Page",
                Parent = parent,
                UrlSegment = "parent/test-page",
                Site = CurrentSite
            };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(parent);
                session.SaveOrUpdate(textPage);
            });
            A.CallTo(() => _urlValidationService.UrlIsValidForWebpage("parent/test-page/nested-page", null))
                .Returns(false);

            string documentUrl = _webpageUrlService.Suggest(textPage,new SuggestParams
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
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Site = CurrentSite };
            var textPage = new BasicMappedWebpage
            {
                Name = "Test Page",
                Parent = parent,
                UrlSegment = "parent/test-page",
                Site = CurrentSite
            };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(parent);
                session.SaveOrUpdate(textPage);
            });
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