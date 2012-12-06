using System.Collections.Generic;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface ISiteService
    {
        List<Site> GetAllSites();
        Site GetSite(int id);
        void AddSite(Site site);
        void SaveSite(Site site);
        void DeleteSite(Site site);
        Site GetCurrentSite();
    }
}