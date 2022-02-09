using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events;
using MrCMS.Website.Caching;

namespace MrCMS.Services
{
    public class ClearHomePageCache : IOnUpdated<Webpage>
    {
        private readonly ICacheManager _cacheManager;

        public ClearHomePageCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Task Execute(OnUpdatedArgs<Webpage> args)
        {
            if (args.Item.Parent?.Id == null) // top level so clear homepage cache
            {
                _cacheManager.Clear();
            }

            return Task.CompletedTask;
        }
    }
}