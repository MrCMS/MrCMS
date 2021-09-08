using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Services.Sitemaps;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Tasks
{
    public class SitemapController : MrCMSUIController
    {
        public const string WriteSitemapUrl = "update-sitemap";
        public const string SitemapUrl = "sitemap.xml";
        private readonly ISitemapService _sitemapService;
        private readonly IGetSitemapPath _getSitemapPath;
        private readonly ICurrentSiteLocator _siteLocator;


        public SitemapController(ISitemapService sitemapService, IGetSitemapPath getSitemapPath, ICurrentSiteLocator siteLocator)
        {
            _sitemapService = sitemapService;
            _getSitemapPath = getSitemapPath;
            _siteLocator = siteLocator;
        }

        [TaskExecutionKeyPasswordAuth]
        [Route(WriteSitemapUrl)]
        public async Task<ContentResult> Update()
        {
            await _sitemapService.WriteSitemap();
            return new ContentResult { Content = "Executed", ContentType = "text/plain" };
        }

        [Route(SitemapUrl)]
        public ActionResult Show()
        {
            var site = _siteLocator.GetCurrentSite();
            if (!_getSitemapPath.FileExists(site))
                return new EmptyResult();

            return PhysicalFile(_getSitemapPath.GetAbsolutePath(site), "application/xml");
        }
    }
}