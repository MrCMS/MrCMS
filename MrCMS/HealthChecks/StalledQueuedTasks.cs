using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class StalledQueuedTasks : HealthCheck
    {
        private readonly ISession _session;

        public StalledQueuedTasks(ISession session)
        {
            _session = session;
        }

        public override string DisplayName
        {
            get { return "Stalled Queued Tasks"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var any = _session.QueryOver<QueuedTask>()
                .Where(
                    task =>
                        task.Status == TaskExecutionStatus.Pending &&
                        task.CreatedOn <= CurrentRequestData.Now.AddMinutes(-30))
                .Any();
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