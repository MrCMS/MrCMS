using Microsoft.EntityFrameworkCore.Internal;
using MrCMS.Entities;
using MrCMS.Events;
using MrCMS.Indexing.Management;
using MrCMS.Services;
using MrCMS.Tasks;
using MrCMS.Website;
using System;
using System.Linq;

namespace MrCMS.DbConfiguration.Configuration
{
    public class UpdateIndicesListener : IOnAdded<SiteEntity>, IOnUpdated<SiteEntity>, IOnDeleted<SiteEntity>
    {
        private readonly IIndexService _indexService;
        private readonly IEndRequestTaskManager _endRequestTaskManager;

        public UpdateIndicesListener(IIndexService indexService, IEndRequestTaskManager endRequestTaskManager)
        {
            _indexService = indexService;
            _endRequestTaskManager = endRequestTaskManager;
        }
        public void QueueTask(Type type, SiteEntity siteEntity, LuceneOperation operation)
        {
            var indexes = _indexService.GetAllIndexManagers();
            if (indexes.Any(x => x.Definition.GetUpdateTypes(operation).Any(updateType => updateType.IsInstanceOfType(siteEntity))))
            {
                var info = new QueuedTaskInfo
                {
                    Data = siteEntity.Id.ToString(),
                    Type = type.MakeGenericType(siteEntity.GetType()),
                    SiteId = siteEntity.Site.Id
                };

                if (!_endRequestTaskManager.GetTasks().OfType<AddLuceneTaskInfo>().Any(task => info.Equals(task.Data)))
                {
                    _endRequestTaskManager.AddTask(new AddLuceneTaskInfo(info));
                }
            }
        }

        private static bool ShouldBeUpdated(SiteEntity siteEntity)
        {
            return siteEntity != null && !(siteEntity is IHaveExecutionStatus);
        }

        public void Execute(OnAddedArgs<SiteEntity> args)
        {
            var siteEntity = args.Item;
            if (siteEntity == null)
            {
                return;
            }

            if (ShouldBeUpdated(siteEntity))
            {
                QueueTask(typeof(InsertIndicesTask<>), siteEntity, LuceneOperation.Insert);
            }
        }

        public void Execute(OnUpdatedArgs<SiteEntity> args)
        {
            var siteEntity = args.Item;
            if (siteEntity == null)
            {
                return;
            }

            if (ShouldBeUpdated(siteEntity))
            {
                QueueTask(typeof(UpdateIndicesTask<>), siteEntity, LuceneOperation.Update);
            }
        }

        public void Execute(OnDeletedArgs<SiteEntity> args)
        {
            var siteEntity = args.Item;
            if (siteEntity == null)
            {
                return;
            }

            if (ShouldBeUpdated(siteEntity))
            {
                QueueTask(typeof(DeleteIndicesTask<>), siteEntity, LuceneOperation.Delete);
            }
        }
    }
}