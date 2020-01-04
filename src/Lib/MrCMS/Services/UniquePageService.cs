using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Documents.Web;
using System.Linq;
using MrCMS.Data;

namespace MrCMS.Services
{
    public class UniquePageService : IUniquePageService
    {
        private readonly IRepository<Webpage> _repository;
        private readonly IGetLiveUrl _getLiveUrl;

        public UniquePageService(IRepository<Webpage> repository, IGetLiveUrl getLiveUrl)
        {
            _repository = repository;
            _getLiveUrl = getLiveUrl;
        }

        public T GetUniquePage<T>()
            where T : Webpage, IUniquePage
        {
            return _repository.Query<T>().FirstOrDefault();
        }

        public string GetUrl<T>(object queryString = null) where T : Webpage, IUniquePage
        {
            var page = GetUniquePage<T>();
            var url = _getLiveUrl.GetUrlSegment(page, true);
            if (queryString != null)
            {
                url += string.Format("?{0}",
                    string.Join("&",
                        new RouteValueDictionary(queryString).Select(
                            pair => string.Format("{0}={1}", pair.Key, pair.Value))));
            }

            return url;

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
            return new RedirectResult(GetUrl<T>(routeValues), isPermanent);
        }
    }
}