using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class UrlHistoryAdminService : IUrlHistoryAdminService
    {
        private readonly ISession _session;

        public UrlHistoryAdminService(ISession session)
        {
            _session = session;
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
            return _session.QueryOver<UrlHistory>().Where(x => x.Site == CurrentRequestData.CurrentSite
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