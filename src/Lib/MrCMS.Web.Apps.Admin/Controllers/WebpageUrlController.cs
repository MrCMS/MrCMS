using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class WebpageUrlController : MrCMSAdminController
    {
        private readonly IWebpageUrlService _webpageUrlService;

        public WebpageUrlController(IWebpageUrlService webpageUrlService)
        {
            _webpageUrlService = webpageUrlService;
        }

        public async Task<string> Suggest(SuggestParams suggestParams)
        {
            return await _webpageUrlService.Suggest(suggestParams);
        }
    }
}