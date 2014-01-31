using MrCMS.Entities;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Tasks
{
    internal class DeleteIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public DeleteIndicesTask(ISession session) : base(session)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Delete(entity);
        }
    }
}