using System.Web.Mvc;
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
        public ContentResult Update()
        {
            _sitemapService.WriteSitemap();
            return new ContentResult { Content = "Executed", ContentType = "text/plain" };
        }

        public ActionResult Show()
        {
            var fileName = _getSitemapPath.GetPath(_site);
            if (_getSitemapPath.FileExists(fileName))
                return File(fileName, "application/xml");
            return new EmptyResult();
        }
    }
}