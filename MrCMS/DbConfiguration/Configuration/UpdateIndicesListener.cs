using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;
using NHibernate.Event;
using Ninject;
using NHibernate.Util;
using System.Linq;

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
        public void OnPostRecreateCollection(PostCollectionRecreateEvent @event)
        {
            var siteEntity = @event.AffectedOwnerOrNull as SiteEntity;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(UpdateIndicesTask<>), siteEntity);
        }

        public void OnPostRemoveCollection(PostCollectionRemoveEvent @event)
        {
            var siteEntity = @event.AffectedOwnerOrNull as SiteEntity;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(UpdateIndicesTask<>), siteEntity);
        }

        public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        {
            var siteEntity = @event.AffectedOwnerOrNull as SiteEntity;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(UpdateIndicesTask<>), siteEntity);
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var siteEntity = @event.Entity as SiteEntity;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(DeleteIndicesTask<>), siteEntity);
        }

        public void OnPostInsert(PostInsertEvent @event)
        {
            var siteEntity = @event.Entity as SiteEntity;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(InsertIndicesTask<>), siteEntity);
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var siteEntity = @event.Entity as SiteEntity;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(UpdateIndicesTask<>), siteEntity);
        }

        public static void QueueTask(Type type, SiteEntity siteEntity)
        {
            var queuedTask = new QueuedTask
                                 {
                                     Data = siteEntity.Id.ToString(),
                                     Type = type.MakeGenericType(siteEntity.GetType()).FullName,
                                     Status = TaskExecutionStatus.Pending,
                                 };
            if (!CurrentRequestData.QueuedTasks.Any(task => task.Data == queuedTask.Data && task.Type == queuedTask.Type))
                CurrentRequestData.QueuedTasks.Add(queuedTask);
        }

        private static bool ShouldBeUpdated(SiteEntity siteEntity)
        {
            return siteEntity != null && !siteEntity.IsDeleted && !(siteEntity is IHaveExecutionStatus);
        }
    }
}