using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Tasks;
using MrCMS.Website;

namespace MrCMS.HealthChecks
{
    public class StalledQueuedTasks : HealthCheck
    {
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly IRepository<QueuedTask> _repository;

        public StalledQueuedTasks(IRepository<QueuedTask> repository, IGetDateTimeNow getDateTimeNow)
        {
            _repository = repository;
            _getDateTimeNow = getDateTimeNow;
        }

        public override string DisplayName => "Stalled Queued Tasks";

        public override async Task<HealthCheckResult> PerformCheck()
        {
            var checkDate = (await _getDateTimeNow.GetLocalNow()).AddMinutes(-30);
            var any = _repository.Readonly()
                .Any(task => task.Status == TaskExecutionStatus.Pending &&
                             task.CreatedOn <= checkDate);
            return any
                ? new HealthCheckResult
                {
                    Messages = new List<string>
                    {
                        "One or more tasks have not been run in the last 30 minutes. " +
                        "Please check that your scheduler is still configured correctly."
                    }
                }
                : HealthCheckResult.Success;
        }
    }
}