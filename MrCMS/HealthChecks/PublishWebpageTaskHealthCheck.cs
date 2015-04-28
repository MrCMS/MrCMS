using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class PublishWebpageTaskHealthCheck : HealthCheck
    {
        private readonly ISession _session;
        private readonly Site _site;

        public PublishWebpageTaskHealthCheck(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public override string DisplayName
        {
            get { return "Page Publisher task setup"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var any = _session.QueryOver<ScheduledTask>().Where(x => x.Type == typeof(PublishScheduledWebpagesTask).FullName && x.Site.Id == _site.Id).Any();
            return !any
                ? new HealthCheckResult
                {
                    Messages = new List<string>
                    {
                        "Publisher task is not set up."
                    }
                }
                : HealthCheckResult.Success;
        }
    }
}