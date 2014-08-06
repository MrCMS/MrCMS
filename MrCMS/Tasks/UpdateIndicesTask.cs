using MrCMS.Entities;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Tasks
{
    internal class UpdateIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public UpdateIndicesTask(ISession session, IIndexService indexService)
            : base(session, indexService)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Update(entity);
        }

        protected override LuceneOperation Operation
        {
            get { return LuceneOperation.Update; }
        }
    }
}