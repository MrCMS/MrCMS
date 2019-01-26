using System.Collections.Generic;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class Sitemap
    {
        public List<SitemapNode> Nodes { get; set; } = new List<SitemapNode>();
    }
}