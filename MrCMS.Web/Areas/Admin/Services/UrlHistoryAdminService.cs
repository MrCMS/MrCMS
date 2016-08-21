using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class UrlHistoryAdminService : IUrlHistoryAdminService
    {
        private readonly IRepository<UrlHistory> _urlHistoryRepository;

        public UrlHistoryAdminService(IRepository<UrlHistory> urlHistoryRepository)
        {
            _urlHistoryRepository = urlHistoryRepository;
        }

        public void Delete(UrlHistory urlHistory)
        {
            if (urlHistory.Webpage != null) urlHistory.Webpage.Urls.Remove(urlHistory);
            _urlHistoryRepository.Delete(urlHistory);
        }

        public void Add(UrlHistory urlHistory)
        {
            if (urlHistory.Webpage != null) urlHistory.Webpage.Urls.Add(urlHistory);
            _urlHistoryRepository.Add(urlHistory);
        }

        public UrlHistory GetByUrlSegment(string url)
        {
            return _urlHistoryRepository.Query().FirstOrDefault(history => history.UrlSegment == url);
        }

        public UrlHistory GetUrlHistoryToAdd(int webpageId)
        {
            return new UrlHistory
            {
                Webpage = new AddPageModel {Id = webpageId}
            };
        }
    }
}