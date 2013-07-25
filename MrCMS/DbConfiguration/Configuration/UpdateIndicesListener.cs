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
            var siteEntity = @event.Entity as SiteEntity;
            if (siteEntity != null) TaskExecutor.ExecuteLater(Create(typeof (UpdateIndicesTask<>), siteEntity));
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var siteEntity = @event.Entity as SiteEntity;
            if (siteEntity != null) TaskExecutor.ExecuteLater(Create(typeof (InsertIndicesTask<>), siteEntity));
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var siteEntity = @event.Entity as SiteEntity;
            if (siteEntity != null) TaskExecutor.ExecuteLater(Create(typeof (DeleteIndicesTask<>), siteEntity));
        }

        private BackgroundTask Create(Type type, SiteEntity siteEntity)
        {
            return Activator.CreateInstance(type.MakeGenericType(siteEntity.GetType()), siteEntity) as BackgroundTask;
        }
    }
}