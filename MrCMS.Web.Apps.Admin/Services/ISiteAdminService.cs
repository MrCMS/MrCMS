using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ISiteAdminService 
    {
        List<Site> GetAllSites();
        Site GetSite(int id);
        void AddSite(Site site, List<SiteCopyOption> options);
        void SaveSite(Site site);
        void DeleteSite(Site site);
    }
}