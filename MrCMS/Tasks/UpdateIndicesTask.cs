using MrCMS.Entities;
using MrCMS.Indexing.Management;
using NHibernate;
using Ninject;

namespace MrCMS.Tasks
{
    internal class UpdateIndicesTask<T> : IndexManagementTask<T> where T : SiteEntity
    {
        public UpdateIndicesTask(ISession session, IKernel kernel)
            : base(session, kernel)
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