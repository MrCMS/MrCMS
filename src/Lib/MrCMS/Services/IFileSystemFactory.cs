using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface IFileSystemFactory
    {
        IFileSystem GetForCurrentSite();
        IFileSystem GetForSite(Site site);
    }
}