using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Widget;
using MrCMS.Website.Caching;

namespace MrCMS.Events.Widgets
{
    public class ClearWidgetsFromCache : OnDataUpdated<Widget>
    {
        private readonly ICacheManager _cacheManager;

        public ClearWidgetsFromCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public override Task Execute(ChangeInfo data)
        {
            var cacheKey = "Widget." + data.EntityId;
            _cacheManager.Clear(cacheKey);
            return Task.CompletedTask;
        }
    }
}