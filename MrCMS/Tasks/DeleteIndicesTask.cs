using MrCMS.Entities;
using MrCMS.Indexing.Management;

namespace MrCMS.Tasks
{
    public class DeleteIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public DeleteIndicesTask(T entity)
            : base(entity)
        {
        }

        protected override T GetObject()
        {
            return Entity;
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Delete(entity);
        }
    }
}