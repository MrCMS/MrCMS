using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class UrlHistoryAdminService : IUrlHistoryAdminService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public UrlHistoryAdminService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public void Delete(UrlHistory urlHistory)
        {
            if (urlHistory.Webpage != null) urlHistory.Webpage.Urls.Remove(urlHistory);
            _session.Transact(session => _session.Delete(urlHistory));
        }

        public void Add(UrlHistory urlHistory)
        {
            if (urlHistory.Webpage != null) urlHistory.Webpage.Urls.Add(urlHistory);
            _session.Transact(session => session.Save(urlHistory));
        }

        public UrlHistory GetByUrlSegment(string url)
        {
            return _session.QueryOver<UrlHistory>().Where(x => x.Site.Id == _site.Id
                && x.UrlSegment.IsInsensitiveLike(url, MatchMode.Exact)).SingleOrDefault();
        }

        public UrlHistory GetByUrlSegmentWithSite(string url, Site site, Webpage page)
        {
            return _session.QueryOver<UrlHistory>().Where(x => x.Site == site && x.Webpage == page
                && x.UrlSegment.IsInsensitiveLike(url, MatchMode.Exact)).SingleOrDefault();
        }

        public UrlHistory GetUrlHistoryToAdd(int webpageId)
        {
            return new UrlHistory
            {
                Webpage = _session.Get<Webpage>(webpageId)
            };
        }
    }
}