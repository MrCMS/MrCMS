using System.Linq;
using MrCMS.Tasks;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.HealthChecks
{
    public class TaskRunnerCheck : HealthCheck
    {
        private readonly ISession _session;

        public TaskRunnerCheck(ISession session)
        {
            _session = session;
        }

        public override string DisplayName
        {
            get { return "Task Runner"; }
        }

        public override HealthCheckResult PerformCheck()
        {
            var tasks = _session.QueryOver<ScheduledTask>().List();
            var stalledTasks = tasks.Where(x => x.LastComplete <= CurrentRequestData.Now.AddSeconds(-(x.EveryXSeconds + 120))).ToList();

            if (stalledTasks.Any())
            {
                var messages = stalledTasks.Select(task => string.Format("{0} has not been ran since {1}", task.TypeName, task.LastComplete)).ToList();
                return new HealthCheckResult
                {
                    Messages = messages,
                    OK = false
                };
            }
            return HealthCheckResult.Success;
        }
    }
}