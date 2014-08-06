using System;
using System.Linq;
using MrCMS.Entities;
using MrCMS.Events;
using MrCMS.Indexing;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndicesListener : IOnAdded<SiteEntity>, IOnUpdated<SiteEntity>, IOnDeleted<SiteEntity>
    {
        public static void QueueTask(Type type, SiteEntity siteEntity, LuceneOperation operation)
        {
            if (IndexingHelper.AnyIndexes(siteEntity, operation))
            {
                var queuedTask = new QueuedTask
                {
                    Data = siteEntity.Id.ToString(),
                    Type = type.MakeGenericType(siteEntity.GetType()).FullName,
                    Status = TaskExecutionStatus.Pending,
                };
                if (
                    !CurrentRequestData.QueuedTasks.Any(
                        task => task.Data == queuedTask.Data && task.Type == queuedTask.Type))
                    CurrentRequestData.QueuedTasks.Add(queuedTask);
            }
        }

        private static bool ShouldBeUpdated(SiteEntity siteEntity)
        {
            return siteEntity != null && !(siteEntity is IHaveExecutionStatus);
        }

        public void Execute(OnAddedArgs<SiteEntity> args)
        {
            var siteEntity = args.Item;
            if (siteEntity == null) return;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof (InsertIndicesTask<>), siteEntity, LuceneOperation.Insert);
        }

        public void Execute(OnUpdatedArgs<SiteEntity> args)
        {
            var siteEntity = args.Item;
            if (siteEntity == null) return;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof(UpdateIndicesTask<>), siteEntity, LuceneOperation.Update);
        }

        public void Execute(OnDeletedArgs<SiteEntity> args)
        {
            var siteEntity = args.Item;
            if (siteEntity == null) return;
            if (ShouldBeUpdated(siteEntity))
                QueueTask(typeof (DeleteIndicesTask<>), siteEntity, LuceneOperation.Delete);
        }
    }
}