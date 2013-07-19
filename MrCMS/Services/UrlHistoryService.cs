using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class UrlHistoryService : IUrlHistoryService
    {
        private readonly ISession _session;

        public UrlHistoryService (ISession session)
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

        public IEnumerable<UrlHistory> GetAllNotForDocument(Webpage document)
        {
            return _session.QueryOver<UrlHistory>().Where(x=>x.Webpage.Id!=document.Id).Cacheable().List();
        }
        public UrlHistory GetByUrlSegment(string url)
        {
            return _session.QueryOver<UrlHistory>().Where(x => x.Site == CurrentRequestData.CurrentSite
                && x.UrlSegment.IsInsensitiveLike(url, MatchMode.Exact)).SingleOrDefault();
        }
    }
}