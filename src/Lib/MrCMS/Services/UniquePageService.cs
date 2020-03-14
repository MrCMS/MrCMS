using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities.Documents.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public Task<T> GetUniquePage<T>()
            where T : Webpage, IUniquePage
        {
            return _repository.Query<T>().FirstOrDefaultAsync();
        }

        public async Task<string> GetUrl<T>(object queryString = null) where T : Webpage, IUniquePage
        {
            var page = await GetUniquePage<T>();
            var url = await _getLiveUrl.GetUrlSegment(page, true);
            if (queryString != null)
            {
                url += string.Format("?{0}",
                    string.Join("&",
                        new RouteValueDictionary(queryString).Select(
                            pair => string.Format("{0}={1}", pair.Key, pair.Value))));
            }

            return url;

        }

        public async Task<RedirectResult> RedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            return await GetRedirectResult<T>(routeValues, false);
        }

        public async Task<RedirectResult> PermanentRedirectTo<T>(object routeValues = null) where T : Webpage, IUniquePage
        {
            return await GetRedirectResult<T>(routeValues, false);
        }

        private async Task<RedirectResult> GetRedirectResult<T>(object routeValues, bool isPermanent) where T : Webpage, IUniquePage
        {
            return new RedirectResult(await GetUrl<T>(routeValues), isPermanent);
        }
    }
}