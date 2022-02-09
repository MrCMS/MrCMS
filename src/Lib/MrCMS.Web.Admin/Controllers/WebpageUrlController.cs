using System.Threading.Tasks;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class WebpageUrlController : MrCMSAdminController
    {
        private readonly IWebpageUrlService _webpageUrlService;
        private readonly ICurrentSiteLocator _currentSiteLocator;

        public WebpageUrlController(IWebpageUrlService webpageUrlService, ICurrentSiteLocator currentSiteLocator)
        {
            _webpageUrlService = webpageUrlService;
            _currentSiteLocator = currentSiteLocator;
        }

        public async Task<string> Suggest(
            SuggestParams suggestParams)
        {
            return await _webpageUrlService.Suggest(_currentSiteLocator.GetCurrentSite().Id, suggestParams);
        }
    }
}