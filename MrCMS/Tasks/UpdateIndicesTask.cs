using MrCMS.Entities;
using MrCMS.Indexing.Management;
using NHibernate;

namespace MrCMS.Tasks
{
    internal class UpdateIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public UpdateIndicesTask(ISession session) : base(session)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Update(entity);
        }
    }
}