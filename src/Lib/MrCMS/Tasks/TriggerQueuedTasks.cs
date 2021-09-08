using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Tasks
{
    public class TriggerQueuedTasks : SchedulableTask
    {
        private readonly ITriggerQueuedTasksOnSites _triggerQueuedTasksOnSites;
        private readonly IStatelessSession _statelessSession;

        public TriggerQueuedTasks(ITriggerQueuedTasksOnSites triggerQueuedTasksOnSites,
            IStatelessSession statelessSession)
        {
            _triggerQueuedTasksOnSites = triggerQueuedTasksOnSites;
            _statelessSession = statelessSession;
        }

        protected override async Task OnExecute()
        {
            var sites = await _statelessSession.Query<Site>().ToListAsync();

            await _triggerQueuedTasksOnSites.Trigger(sites.ToArray());
        }
    }
}