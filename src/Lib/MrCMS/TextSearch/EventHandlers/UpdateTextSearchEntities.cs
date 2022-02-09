using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Events;
using MrCMS.TextSearch.Services;

namespace MrCMS.TextSearch.EventHandlers
{
    public class UpdateTextSearchEntities : IOnAdded<SystemEntity>, IOnUpdated<SystemEntity>, IOnDeleted<SystemEntity>
    {
        private readonly ITextSearchItemUpdater _updater;

        public UpdateTextSearchEntities(ITextSearchItemUpdater updater)
        {
            _updater = updater;
        }

        public async Task Execute(OnAddedArgs<SystemEntity> args)
        {
            await _updater.Add(args.Item);
        }

        public async Task Execute(OnUpdatedArgs<SystemEntity> args)
        {
            await _updater.Update(args.Item);
        }

        public async Task Execute(OnDeletedArgs<SystemEntity> args)
        {
            await _updater.Delete(args.Item);
        }
    }
}