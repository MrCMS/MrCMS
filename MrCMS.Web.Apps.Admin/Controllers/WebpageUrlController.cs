using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
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

        public string Suggest(Webpage parent, 
            //[IoCModelBinder(typeof(SuggestParamsModelBinder))]
            SuggestParams suggestParams) // TODO: model-binding
        {
            return _webpageUrlService.Suggest(parent, suggestParams);
        }
    }
}