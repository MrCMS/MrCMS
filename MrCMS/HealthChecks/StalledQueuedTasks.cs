using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class StalledQueuedTasks : HealthCheck
    {
        private readonly IGetNowForSite _getNowForSite;
        private readonly ISession _session;

        public StalledQueuedTasks(ISession session, IGetNowForSite getNowForSite)
        {
            _session = session;
            _getNowForSite = getNowForSite;
        }

        public override string DisplayName => "Stalled Queued Tasks";

        public override HealthCheckResult PerformCheck()
        {
            var checkDate = _getNowForSite.Now.AddMinutes(-30);
            var any = _session.QueryOver<QueuedTask>()
                .Where(
                    task => task.Status == TaskExecutionStatus.Pending &&
                            task.CreatedOn <= checkDate)
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