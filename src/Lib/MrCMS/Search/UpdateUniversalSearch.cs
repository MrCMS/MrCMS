using System;
using MrCMS.Entities;
using MrCMS.Events;

namespace MrCMS.Search
{
    public class UpdateUniversalSearch : IOnAdded<SystemEntity>, IOnUpdated<SystemEntity>,IOnDeleted<SystemEntity>
    {
        private readonly IUniversalSearchIndexManager _universalSearchIndexManager;

        public UpdateUniversalSearch(IUniversalSearchIndexManager universalSearchIndexManager)
        {
            _universalSearchIndexManager = universalSearchIndexManager;
        }

        public void Execute(OnUpdatedArgs<SystemEntity> args)
        {
            _universalSearchIndexManager.Update(args.Item);
        }
        public void Execute(OnAddedArgs<SystemEntity> args)
        {
            _universalSearchIndexManager.Insert(args.Item);
        }
        public void Execute(OnDeletedArgs<SystemEntity> args)
        {
            _universalSearchIndexManager.Delete(args.Item);
        }
    }
}