using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Entities.Multisite;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class SitesService : ISitesService
    {
        private readonly ISession _session;
        private readonly HttpRequestBase _requestBase;

        public SitesService(ISession session, HttpRequestBase requestBase)
        {
            _session = session;
            _requestBase = requestBase;
        }

        public List<Site> GetAllSites()
        {
            return _session.QueryOver<Site>().Cacheable().List().ToList();
        }

        public Site GetSite(int id)
        {
            return _session.Get<Site>(id);
        }

        public void SaveSite(Site site)
        {
            _session.Transact(session => session.SaveOrUpdate(site));
        }

        public void DeleteSite(Site site)
        {
            _session.Transact(session => session.Delete(site));
        }

        public Site GetCurrentSite()
        {
            var url = _requestBase.Url.ToString();

            var allSites = GetAllSites();
            var site = allSites.FirstOrDefault(s => url.StartsWith(s.BaseUrl));

            return site ?? allSites.FirstOrDefault();
        }
    }
}