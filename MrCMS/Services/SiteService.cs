using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class SiteService : ISiteService
    {
        private readonly ISession _session;
        private readonly ICloneSiteService _cloneSiteService;
        private readonly IIndexService _indexService;

        public SiteService(ISession session, ICloneSiteService cloneSiteService, IIndexService indexService)
        {
            _session = session;
            _cloneSiteService = cloneSiteService;
            _indexService = indexService;
        }

        public List<Site> GetAllSites()
        {
            return _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.Cacheable().List().ToList();
        }

        public Site GetSite(int id)
        {
            return _session.Get<Site>(id);
        }

        public void AddSite(Site site, SiteCopyOptions options)
        {
            _session.Transact(session => session.Save(site));

            if (options.SiteId.HasValue)
            {
                _cloneSiteService.CloneData(site, options);
            }
        }

        public void SaveSite(Site site)
        {
            _session.Transact(session => session.Update(site));
        }

        public void DeleteSite(Site site)
        {
            _session.Transact(session =>
                                  {
                                      site.OnDeleting(session);
                                      session.Delete(site);
                                  });
        }

    }
}