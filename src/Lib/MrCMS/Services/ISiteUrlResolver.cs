using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface ISiteUrlResolver
    {
        string GetCurrentSiteUrl();
        string GetSiteUrl(Site site);
    }
}