using System.IO;
using Microsoft.AspNetCore.Hosting;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Sitemaps
{
    public class GetSitemapPath : IGetSitemapPath
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public GetSitemapPath(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetRelativePath(Site site)
        {
            return $"sitemap-{site.Guid}.xml";
        }

        public string GetRelativePathForPart(Site site, int partNumber)
        {
            return $"sitemap-{site.Guid}-{partNumber}.xml.gz";
        }

        public string GetAbsolutePath(Site site)
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, GetRelativePath(site));
        }

        public string GetAbsolutePathForPart(Site site, int partNumber)
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, GetRelativePathForPart(site, partNumber));
        }

        public bool FileExists(Site site)
        {
            return _hostingEnvironment.WebRootFileProvider.GetFileInfo(GetRelativePath(site)).Exists;
        }
    }
}