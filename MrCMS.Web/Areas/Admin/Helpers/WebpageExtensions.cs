using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Sitemaps;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class WebpageExtensions
    {
        public static bool IsExcludedFromSitemap(this HtmlHelper<Webpage> helper)
        {
            var appender = helper.ViewContext.HttpContext.Get<ISitemapElementAppender>();

            return !appender.ShouldAppend(helper.ViewData.Model);
        }
    }
}