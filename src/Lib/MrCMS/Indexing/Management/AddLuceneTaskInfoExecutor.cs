using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.Indexing.Management
{
    public class AddLuceneTaskInfoExecutor : ExecuteEndRequestBase<AddLuceneTaskInfo, QueuedTaskInfo>
    {
        private readonly IRepository<QueuedTask> _repository;

        public AddLuceneTaskInfoExecutor(IRepository<QueuedTask> repository)
        {
            _repository = repository;
        }

        public override Task Execute(IEnumerable<QueuedTaskInfo> data, CancellationToken token)
        {
            return _repository.AddRange(data.Select(queuedTaskInfo =>
                new QueuedTask
                {
                    Data = queuedTaskInfo.Data,
                    Type = queuedTaskInfo.Type.FullName,
                    Status = TaskExecutionStatus.Pending,
                    SiteId = queuedTaskInfo.SiteId
                }).ToList(), token);
        }
    }
}