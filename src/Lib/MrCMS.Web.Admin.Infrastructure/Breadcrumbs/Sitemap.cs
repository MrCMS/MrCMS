using System.Collections.Generic;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class Sitemap
    {
        public List<SitemapNode> Nodes { get; set; } = new List<SitemapNode>();
    }
}