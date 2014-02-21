using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class EmailSenderTaskCheck : HealthCheck
    {
        private readonly ISession _session;
        private readonly Site _site;

        public EmailSenderTaskCheck(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public override string DisplayName
        {
            get { return "Send email task setup"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var any = _session.QueryOver<ScheduledTask>().Where(x => x.Type == typeof(SendQueuedMessagesTask).FullName && x.Site.Id == _site.Id).Any();
            return !any
                ? new HealthCheckResult
                {
                    Messages = new List<string>
                    {
                        "Email sending task is not set up."
                    }
                }
                : HealthCheckResult.Success;
        }
    }
}