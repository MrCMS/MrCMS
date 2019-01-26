using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using NHibernate;
using System.Linq;

namespace MrCMS.Services
{
    public class GetHomePage : IGetHomePage
    {
        private readonly ISession _session;
        private readonly ICacheInHttpContext _cacheInHttpContext;

        public GetHomePage(ISession session, ICacheInHttpContext cacheInHttpContext)
        {
            _session = session;
            _cacheInHttpContext = cacheInHttpContext;
        }

        public Webpage Get()
        {
            return _cacheInHttpContext.GetForRequest("current.home-page", () => _session.QueryOver<Webpage>()
                .Where(document => document.Parent == null && document.Published)
                .OrderBy(webpage => webpage.DisplayOrder).Asc
                .Take(1)
                .Cacheable()
                .List()
                .FirstOrDefault());
        }
    }
}