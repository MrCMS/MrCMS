using System;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Indexing.Management;
using MrCMS.Services;

namespace MrCMS.Tasks
{
    internal class InsertIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public InsertIndicesTask(IDataReader dataReader, IIndexService indexService, IServiceProvider serviceProvider)
            : base(dataReader, indexService, serviceProvider)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Insert(entity);
        }

        protected override LuceneOperation Operation
        {
            get { return LuceneOperation.Insert; }
        }
    }
}