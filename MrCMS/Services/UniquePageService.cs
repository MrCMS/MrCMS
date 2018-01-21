using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using NHibernate;

namespace MrCMS.Services
{
    public class UniquePageService : IUniquePageService
    {
        private readonly ISession _session;

        public UniquePageService(ISession session)
        {
            _session = session;
        }

        public T GetUniquePage<T>()
            where T : Webpage, IUniquePage
        {
            return _session.QueryOver<T>().Cacheable()
                    .List().FirstOrDefault();
        }

        public RedirectResult RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            return GetRedirectResult<T>(routeValues, false);
        }

        public RedirectResult PermanentRedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            return GetRedirectResult<T>(routeValues, false);
        }

        private RedirectResult GetRedirectResult<T>(object routeValues, bool isPermanent) where T : Webpage, IUniquePage
        {
            var page = GetUniquePage<T>();
            string url = page != null ? string.Format("/{0}", page.LiveUrlSegment) : "/";
            if (routeValues != null)
            {
                var dictionary = new RouteValueDictionary(routeValues);
                url += string.Format("?{0}",
                    string.Join("&",
                        dictionary.Select(
                            pair => string.Format("{0}={1}", pair.Key, pair.Value))));
            }
            return new RedirectResult(url, isPermanent);
        }
    }
}