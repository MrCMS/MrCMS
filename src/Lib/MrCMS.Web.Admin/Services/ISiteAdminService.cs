using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ISiteAdminService 
    {
        Task<IList<Site>> GetAllSites();
        Task<Site> GetSite(int id);
        Task AddSite(AddSiteModel model, List<SiteCopyOption> options);
        Task<UpdateSiteModel> GetEditModel(int id);
        Task<IList<RedirectedDomain>> GetRedirectedDomains(int id);
        Task SaveSite(UpdateSiteModel model);
        Task DeleteSite(int id);
    }
}