using System;
using MrCMS.Entities;
using MrCMS.Tasks;
using NHibernate.Event;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndicesListener : IPostUpdateEventListener, IPostInsertEventListener, IPostDeleteEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            TaskExecutor.ExecuteLater(Create(typeof (UpdateIndicesTask<>), (@event.Entity as SiteEntity)));
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            TaskExecutor.ExecuteLater(Create(typeof (InsertIndicesTask<>), (@event.Entity as SiteEntity)));
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            TaskExecutor.ExecuteLater(Create(typeof (DeleteIndicesTask<>), (@event.Entity as SiteEntity)));
        }

        private BackgroundTask Create(Type type, SiteEntity siteEntity)
        {
            return Activator.CreateInstance(type.MakeGenericType(siteEntity.GetType()), siteEntity) as BackgroundTask;
        }
    }
}