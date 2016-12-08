using System.IO;
using System.Web;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Sitemaps
{
    public class GetSitemapPath : IGetSitemapPath
    {
        private readonly HttpServerUtilityBase _server;

        public GetSitemapPath(HttpServerUtilityBase server)
        {
            _server = server;
        }
        public string GetPath(Site site)
        {
            return _server.MapPath($"~/App_Data/sitemap-{site.Id}.xml");
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}