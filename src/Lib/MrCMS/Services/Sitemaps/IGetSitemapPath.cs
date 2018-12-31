using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Sitemaps
{
    public interface IGetSitemapPath
    {
        string GetRelativePath(Site site);
        string GetAbsolutePath(Site site);
        bool FileExists(Site site);
    }
}