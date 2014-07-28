using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Areas.Admin.Controllers;
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

            _webpageUrlController.Suggest(textPage, "test", typeof (TextPage).FullName, null, true);

            A.CallTo(() => _webpageUrlService.Suggest("test", textPage, typeof(TextPage).FullName, null, true)).MustHaveHappened();
        }

        [Fact]
        public void WebpageController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        {
            var textPage = new TextPage();
            A.CallTo(() => _webpageUrlService.Suggest("test", textPage, typeof(TextPage).FullName, null, true)).Returns("test/result");

            string url = _webpageUrlController.Suggest(textPage, "test", typeof(TextPage).FullName, null, true);

            url.Should().BeEquivalentTo("test/result");
        }

    }
}