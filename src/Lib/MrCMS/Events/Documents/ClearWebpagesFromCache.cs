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

        public void Execute(OnUpdatedArgs<Webpage> args)
        {
            var cacheKey = "Webpage." + args.Item.Id;
            _cacheManager.Clear(cacheKey);
        }
    }
}