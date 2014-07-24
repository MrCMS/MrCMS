using MrCMS.Entities;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Tasks
{
    internal class DeleteIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public DeleteIndicesTask(ISession session, IIndexService indexService)
            : base(session, indexService)
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