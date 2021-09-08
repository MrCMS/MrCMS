using FakeItEasy;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Tests.Website.Controllers.Builders
{
    public class WebpageControllerBuilder
    {
        private IWebpageUIService _webpageUiService = A.Fake<IWebpageUIService>();
        
        public WebpageController Build()
        {
            return new WebpageController(_webpageUiService);
        }

        public WebpageControllerBuilder WithUiService(IWebpageUIService uiService)
        {
            _webpageUiService = uiService;
            return this;
        }
    }
}