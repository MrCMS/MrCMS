using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
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