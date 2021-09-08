using MrCMS.Entities.Multisite;

namespace MrCMS.Settings
{
    public interface IConfigurationProviderFactory
    {
        IConfigurationProvider GetForSite(Site site);
    }
}