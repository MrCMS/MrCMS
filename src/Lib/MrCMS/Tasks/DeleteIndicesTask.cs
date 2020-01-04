using System;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Indexing.Management;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    internal class DeleteIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public DeleteIndicesTask(IDataReader reader, IIndexService indexService, IServiceProvider serviceProvider)
            : base(reader, indexService, serviceProvider)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Delete(entity);
        }

        protected override LuceneOperation Operation
        {
            get { return LuceneOperation.Delete; }
        }
    }
}