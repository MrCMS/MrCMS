using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Search
{
    public class AddUniversalSearchTaskInfoExecutor : ExecuteEndRequestBase<AddUniversalSearchTaskInfo, UniversalSearchIndexData>
    {
        private readonly IStatelessSession _session;
        private readonly Site _site;

        public AddUniversalSearchTaskInfoExecutor(IStatelessSession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public override void Execute(IEnumerable<UniversalSearchIndexData> data)
        {
            _session.Transact(session =>
            {
                foreach (var indexData in data)
                {
                    session.Insert(new QueuedTask
                    {
                        Data = JsonConvert.SerializeObject(indexData),
                        Type = typeof(UniversalSearchIndexTask).FullName,
                        Status = TaskExecutionStatus.Pending,
                        Site = GetSite(indexData),
                        CreatedOn = CurrentRequestData.Now,
                        UpdatedOn = CurrentRequestData.Now
                    });
                }
            });
        }

        private Site GetSite(UniversalSearchIndexData indexData)
        {
            var item = indexData.UniversalSearchItem;
            var entity = _session.Get(item.SystemType, item.Id) as SiteEntity;
            return entity == null || entity.Site == null
                ? _session.Get<Site>(_site.Id)
                : _session.Get<Site>(entity.Site.Id);
        }
    }
}