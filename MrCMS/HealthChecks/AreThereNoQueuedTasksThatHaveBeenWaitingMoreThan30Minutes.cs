using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class AreThereNoQueuedTasksThatHaveBeenWaitingMoreThan30Minutes : HealthCheck
    {
        private readonly ISession _session;
        private readonly Site _site;

        public AreThereNoQueuedTasksThatHaveBeenWaitingMoreThan30Minutes(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public override string DisplayName
        {
            get { return "Are there no queued tasks that have been waiting more than 30 minutes?"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var any= _session.QueryOver<QueuedTask>()
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