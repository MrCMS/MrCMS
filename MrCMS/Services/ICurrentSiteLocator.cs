using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface ICurrentSiteLocator
    {
        Site GetCurrentSite();
    }
}