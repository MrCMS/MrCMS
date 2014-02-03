using MrCMS.Entities;
using MrCMS.Indexing.Management;
using NHibernate;
using Ninject;

namespace MrCMS.Tasks
{
    internal class DeleteIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public DeleteIndicesTask(ISession session, IKernel kernel)
            : base(session, kernel)
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