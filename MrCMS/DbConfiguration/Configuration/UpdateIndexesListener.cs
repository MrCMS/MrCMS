using MrCMS.Entities;
using MrCMS.Tasks;
using NHibernate.Event;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndexesListener : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            TaskExecutor.ExecuteLater(new UpdateIndexesTask(@event.Entity as SiteEntity));
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            TaskExecutor.ExecuteLater(new InsertIndexesTask(@event.Entity as SiteEntity));
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            TaskExecutor.ExecuteLater(new DeleteIndexesTask(@event.Entity as SiteEntity));
        }
    }
}