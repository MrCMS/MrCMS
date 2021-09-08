using System.Threading.Tasks;
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

        public Task Execute(OnUpdatedArgs<Widget> args)
        {
            _cacheManager.Clear();
            return Task.CompletedTask;
        }
    }
}