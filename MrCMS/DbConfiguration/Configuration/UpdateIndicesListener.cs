using System;
using MrCMS.Entities;
using MrCMS.Tasks;
using NHibernate.Event;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndicesListener :
        IPostUpdateEventListener,
        IPostInsertEventListener,
        IPostDeleteEventListener,
        IPostCollectionUpdateEventListener,
        IPostCollectionRemoveEventListener,
        IPostCollectionRecreateEventListener
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

        public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        {
            var siteEntity = @event.AffectedOwnerOrNull as SiteEntity;
            if (siteEntity != null) TaskExecutor.ExecuteLater(Create(typeof(UpdateIndicesTask<>), siteEntity));
        }

        public void OnPostRemoveCollection(PostCollectionRemoveEvent @event)
        {
            var siteEntity = @event.AffectedOwnerOrNull as SiteEntity;
            if (siteEntity != null) TaskExecutor.ExecuteLater(Create(typeof(UpdateIndicesTask<>), siteEntity));
        }

        public void OnPostRecreateCollection(PostCollectionRecreateEvent @event)
        {
            var siteEntity = @event.AffectedOwnerOrNull as SiteEntity;
            if (siteEntity != null) TaskExecutor.ExecuteLater(Create(typeof(UpdateIndicesTask<>), siteEntity));
        }

        private BackgroundTask Create(Type type, SiteEntity siteEntity)
        {
            return Activator.CreateInstance(type.MakeGenericType(siteEntity.GetType()), siteEntity) as BackgroundTask;
        }
    }
}