using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website.Caching;

namespace MrCMS.Events.Documents
{
    public class ClearWebpagesFromCache : IOnUpdated<Webpage>
    {
        private readonly ICacheManager _cacheManager;

        public ClearWebpagesFromCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Task Execute(OnUpdatedArgs<Webpage> args)
        {
            _cacheManager.Clear();
            return Task.CompletedTask;
        }
    }
}