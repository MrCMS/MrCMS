using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Controllers;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Controllers
{
    public class WebpageUrlControllerTests
    {
        private readonly IWebpageUrlService _webpageUrlService;
        private readonly WebpageUrlController _webpageUrlController;
        private readonly ICurrentSiteLocator _currentSiteLocator;

        public WebpageUrlControllerTests()
        {
            _webpageUrlService = A.Fake<IWebpageUrlService>();
            _currentSiteLocator = A.Fake<ICurrentSiteLocator>();
            _webpageUrlController = new WebpageUrlController(_webpageUrlService, _currentSiteLocator);
        }


        [Fact]
        public async Task WebpageController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        {
            var suggestParams = new SuggestParams();
            await _webpageUrlController.Suggest(suggestParams);

            A.CallTo(() => _webpageUrlService.Suggest(_currentSiteLocator.GetCurrentSite().Id, suggestParams))
                .MustHaveHappened();
        }

        [Fact]
        public async Task WebpageController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var suggestParams = new SuggestParams();
            A.CallTo(() => _webpageUrlService.Suggest(_currentSiteLocator.GetCurrentSite().Id, suggestParams))
                .Returns("test/result");

            string url = await _webpageUrlController.Suggest(suggestParams);

            url.Should().BeEquivalentTo("test/result");
        }
    }
}