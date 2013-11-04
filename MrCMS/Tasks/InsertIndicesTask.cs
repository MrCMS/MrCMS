using MrCMS.Entities;
using MrCMS.Indexing.Management;

namespace MrCMS.Tasks
{
    internal class InsertIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public InsertIndicesTask(T entity)
            : base(entity)
        {
        }
        protected override void ExecuteLogic(IIndexManagerBase manager, T entity)
        {
            manager.Insert(entity);
        }
    }
}