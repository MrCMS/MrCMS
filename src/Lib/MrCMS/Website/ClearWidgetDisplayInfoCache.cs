using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Events;
using MrCMS.Website.Caching;

namespace MrCMS.Website
{
    public class ClearWidgetDisplayInfoCache : IOnAdded<Widget>, IOnUpdated<Widget>, IOnDeleted<Widget>,
        IOnAdded<LayoutArea>, IOnUpdated<LayoutArea>, IOnDeleted<LayoutArea>
    {
        private readonly ICacheManager _cacheManager;

        public ClearWidgetDisplayInfoCache(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public Task Execute(OnAddedArgs<Widget> args)
        {
            return ClearDisplayInfo();
        }

        public Task Execute(OnUpdatedArgs<Widget> args)
        {
            return ClearDisplayInfo();
        }

        public Task Execute(OnDeletedArgs<Widget> args)
        {
            return ClearDisplayInfo();
        }

        public Task Execute(OnUpdatedArgs<LayoutArea> args)
        {
            return ClearDisplayInfo();
        }

        public Task Execute(OnDeletedArgs<LayoutArea> args)
        {
            return ClearDisplayInfo();
        }

        public Task Execute(OnAddedArgs<LayoutArea> args)
        {
            return ClearDisplayInfo();
        }


        private Task ClearDisplayInfo()
        {
            _cacheManager.Clear();
            return Task.CompletedTask;
        }
    }
}