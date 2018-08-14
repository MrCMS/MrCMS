using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
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
        private readonly Site _site;


        public SitemapController(ISitemapService sitemapService, IGetSitemapPath getSitemapPath, Site site)
        {
            _sitemapService = sitemapService;
            _getSitemapPath = getSitemapPath;
            _site = site;
        }

        [TaskExecutionKeyPasswordAuth]
        [Route(WriteSitemapUrl)]
        public ContentResult Update()
        {
            _sitemapService.WriteSitemap();
            return new ContentResult { Content = "Executed", ContentType = "text/plain" };
        }

        [Route(SitemapUrl)]
        public ActionResult Show()
        {
            if (!_getSitemapPath.FileExists(_site))
                return new EmptyResult();

            return PhysicalFile(_getSitemapPath.GetAbsolutePath(_site), "application/xml");
        }
    }
}