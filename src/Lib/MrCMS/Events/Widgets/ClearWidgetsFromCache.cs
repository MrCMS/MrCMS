using MrCMS.Entities.Widget;
using MrCMS.Website.Caching;

namespace MrCMS.Events.Widgets
{
    public class ClearWidgetsFromCache : IOnUpdated<Widget>
    {
        private readonly ICacheManager _cacheManager;

        public ClearWidgetsFromCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void Execute(OnUpdatedArgs<Widget> args)
        {
            var cacheKey = "Widget." + args.Item.Id;
            _cacheManager.Clear(cacheKey);
        }
    }
}