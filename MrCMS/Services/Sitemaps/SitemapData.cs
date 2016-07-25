using System;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapData
    {
        public DateTime? PublishOn { get; set; }
        public bool RequiresSSL { get; set; }
        public string Url { get; set; }

        public string AbsoluteUrl { get; private set; }

        public void SetAbsoluteUrl(SiteSettings siteSettings, Site site, string homepageUrl)
        {
            var scheme = RequiresSSL || siteSettings.SSLEverywhere
                ? "https://"
                : "http://";
            var authority = site.BaseUrl;
            if (authority.EndsWith("/"))
                authority = authority.TrimEnd('/');

            AbsoluteUrl = string.Format("{0}{1}/{2}", scheme, authority, Url == homepageUrl ? string.Empty : Url);
        }

    }
}