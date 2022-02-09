using System;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapData
    {
        public int WebpageId { get; set; }
        public DateTime? PublishOn { get; set; }
        public bool RequiresSSL { get; set; }
        public string Url { get; set; }

        public string AbsoluteUrl { get; private set; }

        public void SetAbsoluteUrl(Site site, string homepageUrl)
        {
            var scheme = RequiresSSL
                ? "https://"
                : "http://";
            var authority = site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');
            var url = Url.TrimStart('/');

            AbsoluteUrl = $"{scheme}{authority}/{(url == homepageUrl ? string.Empty : url)}";
        }

    }
}