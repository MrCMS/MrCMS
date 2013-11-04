using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using MrCMS.Entities.Documents;
using System.Linq;
namespace MrCMS.Services
{
    public class UrlHistoryService : IUrlHistoryService
    {
        private readonly ISession _session;

        public UrlHistoryService(ISession session)
        {
            _session = session;
        }

        public void Delete(UrlHistory urlHistory)
        {
            _session.Transact(session => _session.Delete(urlHistory));
        }

        public void Add(UrlHistory urlHistory)
        {
            _session.Transact(session => session.Save(urlHistory));
        }

        public IEnumerable<UrlHistory> GetAllOtherUrls(Webpage document)
        {
            var urlHistory = _session.QueryOver<UrlHistory>().Where(x => x.Webpage.Id != document.Id).Cacheable().List();
            var urls = _session.QueryOver<Document>().Where(x => x.Id != document.Id).Cacheable().List();
            foreach (var url in urls)
            {
                if (!urlHistory.Any(x => x.UrlSegment == url.UrlSegment))
                    urlHistory.Add(new UrlHistory() { UrlSegment = url.UrlSegment, Webpage = document });
            }
            return urlHistory;
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
    }
}