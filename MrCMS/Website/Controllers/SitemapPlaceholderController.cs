using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Controllers
{
    public class SitemapPlaceholderController : MrCMSUIController
    {
        private readonly ISitemapPlaceholderUIService _sitemapPlaceholderUIService;

        public SitemapPlaceholderController(ISitemapPlaceholderUIService sitemapPlaceholderUIService)
        {
            _sitemapPlaceholderUIService = sitemapPlaceholderUIService;
        }

        public RedirectResult Show(SitemapPlaceholder page)
        {
            return _sitemapPlaceholderUIService.Redirect(page);
        }
    }
}