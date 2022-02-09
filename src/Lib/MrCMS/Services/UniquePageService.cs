using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Documents.Web;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace MrCMS.Services
{
    public class UniquePageService : IUniquePageService
    {
        private readonly ISession _session;
        private readonly IGetLiveUrl _getLiveUrl;

        public UniquePageService(ISession session, IGetLiveUrl getLiveUrl)
        {
            _session = session;
            _getLiveUrl = getLiveUrl;
        }

        public async Task<T> GetUniquePage<T>()
            where T : Webpage, IUniquePage =>
            await _session.Query<T>().WithOptions(options => options.SetCacheable(true)).FirstOrDefaultAsync();

        public async Task<string> GetUrl<T>(object queryString = null) where T : Webpage, IUniquePage
        {
            var page = await GetUniquePage<T>();
            var url = await _getLiveUrl.GetUrlSegment(page, true);
            if (queryString != null)
            {
                url +=
                    $"?{string.Join("&", new RouteValueDictionary(queryString).Select(pair => $"{pair.Key}={pair.Value}"))}";
            }

            return url;
        }

        public async Task<string> GetAbsoluteUrl<T>(object queryString = null) where T : Webpage, IUniquePage
        {
            var page = await GetUniquePage<T>();
            var url = await _getLiveUrl.GetAbsoluteUrl(page);
            if (queryString != null)
            {
                url +=
                    $"?{string.Join("&", new RouteValueDictionary(queryString).Select(pair => $"{pair.Key}={pair.Value}"))}";
            }

            return url;
        }

        public Task<RedirectResult> RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            return GetRedirectResult<T>(routeValues, false);
        }

        public async Task<RedirectResult> PermanentRedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            return await GetRedirectResult<T>(routeValues, false);
        }

        private async Task<RedirectResult> GetRedirectResult<T>(object routeValues, bool isPermanent) where T : Webpage, IUniquePage
        {
            return new(await GetUrl<T>(routeValues), isPermanent);
        }
    }
}