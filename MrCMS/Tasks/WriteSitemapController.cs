using System.Web.Mvc;
using MrCMS.Services.Sitemaps;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    public class WriteSitemapController : MrCMSUIController
    {
        public const string WriteSitemapUrl = "update-sitemap";
        private readonly ISitemapService _sitemapService;

        public WriteSitemapController(ISitemapService sitemapService)
        {
            _sitemapService = sitemapService;
        }

        [TaskExecutionKeyPasswordAuth]
        public ContentResult Execute()
        {
            _sitemapService.WriteSitemap();
            return new ContentResult {Content = "Executed", ContentType = "text/plain"};
        }
    }
}