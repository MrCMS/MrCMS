using System.Collections.Generic;
using MrCMS.DbConfiguration;
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
        private readonly ISession _session;

        public AddUniversalSearchTaskInfoExecutor(ISession session)
        {
            _session = session;
        }

        public override void Execute(IEnumerable<UniversalSearchIndexData> data)
        {
            using (new SiteFilterDisabler(_session))
            {
                _session.Transact(session =>
                {
                    foreach (var indexData in data)
                    {
                        session.Save(new QueuedTask
                        {
                            Data = JsonConvert.SerializeObject(indexData),
                            Type = typeof(UniversalSearchIndexTask).FullName,
                            Status = TaskExecutionStatus.Pending,
                            Site = GetSite(indexData)
                        });
                    }
                });
            }
        }

        private Site GetSite(UniversalSearchIndexData indexData)
        {
            var item = indexData.UniversalSearchItem;
            var entity = _session.Get(item.SystemType, item.Id) as SiteEntity;
            return entity == null || entity.Site == null ? null : _session.Get<Site>(entity.Site.Id);
        }
    }
}