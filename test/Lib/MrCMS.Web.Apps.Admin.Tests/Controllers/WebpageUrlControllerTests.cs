using FakeItEasy;
using FluentAssertions;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Core.Pages;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class WebpageUrlControllerTests
    {
        // todo - rewrite tests and refactor

        //private readonly IWebpageUrlService _webpageUrlService;
        //private readonly WebpageUrlController _webpageUrlController;

        //public WebpageUrlControllerTests()
        //{
        //    _webpageUrlService = A.Fake<IWebpageUrlService>();
        //    _webpageUrlController = new WebpageUrlController(_webpageUrlService);
        //}


        //[Fact]
        //public void WebpageController_SuggestDocumentUrl_ShouldCallGetDocumentUrl()
        //{
        //    var suggestParams = new SuggestParams();
        //    _webpageUrlController.Suggest(suggestParams);

        //    A.CallTo(() => _webpageUrlService.Suggest(suggestParams)).MustHaveHappened();
        //}

        //[Fact]
        //public void WebpageController_SuggestDocumentUrl_ShouldReturnTheResultOfGetDocumentUrl()
        //{
        //    var suggestParams = new SuggestParams();
        //    A.CallTo(() => _webpageUrlService.Suggest(suggestParams)).Returns("test/result");

        //    string url = _webpageUrlController.Suggest(suggestParams);

        //    url.Should().BeEquivalentTo("test/result");
        //}

    }
}