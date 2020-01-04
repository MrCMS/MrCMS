using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website.Caching;

namespace MrCMS.Events.Documents
{
    public class ClearWebpagesFromCache : OnDataUpdated<Webpage>
    {
        private readonly ICacheManager _cacheManager;

        public ClearWebpagesFromCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public override Task Execute(ChangeInfo data)
        {
            var cacheKey = $"Webpage.{data.EntityId}";
            _cacheManager.Clear(cacheKey);
            return Task.CompletedTask;
        }

    }
}