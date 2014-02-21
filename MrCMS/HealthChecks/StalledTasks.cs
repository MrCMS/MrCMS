using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class StalledTasks : HealthCheck
    {
        private readonly ISession _session;
        private readonly Site _site;

        public StalledTasks(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public override string DisplayName
        {
            get { return "Stalled tasks"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var any = _session.QueryOver<QueuedTask>()
                .Where(
                    task =>
                        task.Status == TaskExecutionStatus.Pending &&
                        task.CreatedOn <= CurrentRequestData.Now.AddMinutes(-30) && task.Site.Id == _site.Id)
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