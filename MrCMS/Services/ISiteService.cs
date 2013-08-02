using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ISiteService 
    {
        List<Site> GetAllSites();
        Site GetSite(int id);
        void AddSite(Site site, SiteCopyOptions options);
        void SaveSite(Site site);
        void DeleteSite(Site site);
    }
}