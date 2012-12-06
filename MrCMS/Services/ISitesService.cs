using System.Collections.Generic;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface ISitesService
    {
        List<Site> GetAllSites();
        Site GetSite(int id);
        void SaveSite(Site site);
        void DeleteSite(Site site);
        Site GetCurrentSite();
    }
}