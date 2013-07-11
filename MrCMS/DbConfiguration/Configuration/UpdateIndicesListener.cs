using MrCMS.Entities;
using MrCMS.Tasks;
using NHibernate.Event;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndicesListener : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            TaskExecutor.ExecuteLater(new UpdateIndicesTask(@event.Entity as SiteEntity));
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            TaskExecutor.ExecuteLater(new InsertIndicesTask(@event.Entity as SiteEntity));
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            TaskExecutor.ExecuteLater(new DeleteIndicesTask(@event.Entity as SiteEntity));
        }
    }
}