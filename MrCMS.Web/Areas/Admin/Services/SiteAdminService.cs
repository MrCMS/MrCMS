using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services.CloneSite;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class SiteAdminService : ISiteAdminService
    {
        private readonly ICloneSiteService _cloneSiteService;
        private readonly IRepository<Site> _siteRepository;

        public SiteAdminService(IRepository<Site> siteRepository, ICloneSiteService cloneSiteService)
        {
            _siteRepository = siteRepository;
            _cloneSiteService = cloneSiteService;
        }

        public List<Site> GetAllSites()
        {
            return _siteRepository.Query().OrderBy(site => site.Name).ToList();
        }

        public Site GetSite(int id)
        {
            return _siteRepository.Get(id);
        }

        public void AddSite(Site site, List<SiteCopyOption> options)
        {
            _siteRepository.Add(site);

            _cloneSiteService.CloneData(site, options);
        }

        public void UpdateSite(Site site)
        {
            _siteRepository.Update(site);
        }

        public void DeleteSite(Site site)
        {
            _siteRepository.Delete(site);
        }
    }
}