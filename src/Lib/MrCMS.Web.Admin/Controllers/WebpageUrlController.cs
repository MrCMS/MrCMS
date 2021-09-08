using System.Threading.Tasks;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class WebpageUrlController : MrCMSAdminController
    {
        private readonly IWebpageUrlService _webpageUrlService;

        public WebpageUrlController(IWebpageUrlService webpageUrlService)
        {
            _webpageUrlService = webpageUrlService;
        }

        public async Task<string> Suggest(
            SuggestParams suggestParams) 
        {
            return await _webpageUrlService.Suggest(suggestParams);
        }
    }
}