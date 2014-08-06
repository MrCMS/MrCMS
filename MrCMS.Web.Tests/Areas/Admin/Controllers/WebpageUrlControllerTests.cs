using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class WebpageUrlControllerTests
    {
        private readonly IWebpageUrlService _webpageUrlService;
        private readonly WebpageUrlController _webpageUrlController;

        public WebpageUrlControllerTests()
        {
            _webpageUrlService = A.Fake<IWebpageUrlService>();
            _webpageUrlController = new WebpageUrlController(_webpageUrlService);
        }


        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        {
            var textPage = new TextPage();

            var suggestParams = new SuggestParams();
            _webpageUrlController.Suggest(textPage, suggestParams);

            A.CallTo(() => _webpageUrlService.Suggest(textPage,suggestParams)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var textPage = new TextPage();
            var suggestParams = new SuggestParams();
            A.CallTo(() => _webpageUrlService.Suggest(textPage, suggestParams)).Returns("test/result");

            string url = _webpageUrlController.Suggest(textPage, suggestParams);

            url.Should().BeEquivalentTo("test/result");
        }

    }
}