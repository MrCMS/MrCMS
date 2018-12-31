using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Sitemaps
{
    public class GetSitemapPath : IGetSitemapPath
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public GetSitemapPath(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetRelativePath(Site site)
        {
            return $"App_Data\\sitemap-{site.Id}.xml";
        }

        public string GetAbsolutePath(Site site)
        {
            return Path.Combine(_hostingEnvironment.ContentRootPath, GetRelativePath(site));
        }

        public bool FileExists(Site site)
        {
            return _hostingEnvironment.ContentRootFileProvider.GetFileInfo(GetRelativePath(site)).Exists;
        }
    }
}