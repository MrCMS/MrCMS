using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Sitemaps
{
    public interface IGetSitemapPath
    {
        string GetPath(Site site);
        bool FileExists(string path);
    }
}