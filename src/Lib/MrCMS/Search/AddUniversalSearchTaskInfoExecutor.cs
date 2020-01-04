using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Tasks;
using MrCMS.Website;
using Newtonsoft.Json;

namespace MrCMS.Search
{
    public class AddUniversalSearchTaskInfoExecutor : ExecuteEndRequestBase<AddUniversalSearchTaskInfo, UniversalSearchIndexData>
    {
        private readonly IRepository<QueuedTask> _repository;
        private readonly Site _site;

        public AddUniversalSearchTaskInfoExecutor(IRepository<QueuedTask> repository, Site site)
        {
            _repository = repository;
            _site = site;
        }

        public override Task Execute(IEnumerable<UniversalSearchIndexData> data, CancellationToken token)
        {
            return _repository.AddRange(data.Select(indexData => new QueuedTask
            {
                Data = JsonConvert.SerializeObject(indexData),
                Type = typeof(UniversalSearchIndexTask).FullName,
                Status = TaskExecutionStatus.Pending,
                SiteId = _site.Id
            }).ToList(), token);
        }

        //private Site GetSite(UniversalSearchIndexData indexData)
        //{
        //    var item = indexData.UniversalSearchItem;
        //    var entity = _session.Get(item.SystemType, item.Id) as SiteEntity;
        //    return entity == null || entity.Site == null
        //        ? _session.Get<Site>(_site.Id)
        //        : _session.Get<Site>(entity.Site.Id);
        //}
    }
}