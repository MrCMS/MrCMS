using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.DbConfiguration.Configuration
{
    [EndRequestExecutionPriority(1)]
    public class AddLuceneTaskInfo : EndRequestTask<QueuedTaskInfo>
    {
        public AddLuceneTaskInfo(QueuedTaskInfo data)
            : base(data)
        {
        }
    }
    public class AddLuceneTaskInfoExecutor : ExecuteEndRequestBase<AddLuceneTaskInfo, QueuedTaskInfo>
    {
        private readonly ISession _session;

        public AddLuceneTaskInfoExecutor(ISession session)
        {
            _session = session;
        }

        public override void Execute(IEnumerable<QueuedTaskInfo> data)
        {
            _session.Transact(session =>
            {
                foreach (var queuedTaskInfo in data)
                {
                    session.Save(new QueuedTask
                    {
                        Data = queuedTaskInfo.Data,
                        Type = queuedTaskInfo.Type.FullName,
                        Status = TaskExecutionStatus.Pending,
                        Site = _session.Get<Site>(queuedTaskInfo.SiteId)
                    });
                }
            });
        }
    }
}