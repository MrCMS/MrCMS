using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ISiteAdminService 
    {
        List<Site> GetAllSites();
        Site GetSite(int id);
        Task AddSite(AddSiteModel model, List<SiteCopyOption> options);
        UpdateSiteModel GetEditModel(int id);
        IList<RedirectedDomain> GetRedirectedDomains(int id);
        Task SaveSite(UpdateSiteModel model);
        Task DeleteSite(int id);
    }
}