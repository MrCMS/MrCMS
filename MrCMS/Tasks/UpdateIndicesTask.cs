using MrCMS.Entities;
using MrCMS.Indexing.Management;

namespace MrCMS.Tasks
{
    public class UpdateIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public UpdateIndicesTask(T entity)
            : base(entity)
        {
        }

        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Update(entity);
        }
    }
}