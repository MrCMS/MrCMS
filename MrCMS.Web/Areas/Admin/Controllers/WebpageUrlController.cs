using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageUrlController : MrCMSAdminController
    {
        private readonly IWebpageUrlService _webpageUrlService;

        public WebpageUrlController(IWebpageUrlService webpageUrlService)
        {
            _webpageUrlService = webpageUrlService;
        }

        public string Suggest(Webpage parent, string pageName, string documentType, int? template, bool useHierarchy = true)
        {
            return _webpageUrlService.Suggest(pageName, parent, documentType, template, useHierarchy);
        }
    }
}