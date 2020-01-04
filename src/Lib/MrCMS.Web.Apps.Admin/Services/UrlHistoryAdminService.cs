using AutoMapper;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models;

using NHibernate.Criterion;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class UrlHistoryAdminService : IUrlHistoryAdminService
    {
        private readonly ISession _session;
        private readonly Site _site;
        private readonly IMapper _mapper;

        public UrlHistoryAdminService(ISession session, Site site, IMapper mapper)
        {
            _session = session;
            _site = site;
            _mapper = mapper;
        }

        public UrlHistory Delete(int id)
        {
            var urlHistory = _session.Get<UrlHistory>(id);
            if (urlHistory == null)
            {
                return null;
            }

            urlHistory.Webpage?.Urls.Remove(urlHistory);
            _session.Transact(session => _session.Delete(urlHistory));
            return urlHistory;
        }

        public void Add(AddUrlHistoryModel model)
        {
            var urlHistory = _mapper.Map<UrlHistory>(model);
            urlHistory.Webpage?.Urls.Add(urlHistory);
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

        public AddUrlHistoryModel GetUrlHistoryToAdd(int webpageId)
        {
            return new AddUrlHistoryModel
            {
                WebpageId = webpageId
            };
        }
    }
}