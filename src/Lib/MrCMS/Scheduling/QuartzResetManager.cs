using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl.Matchers;

namespace MrCMS.Scheduling
{
    public class QuartzResetManager : IQuartzResetManager
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzResetManager(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }
        public async Task ResetErroredTriggers(CancellationToken cancellationToken)
        {
            var schedulers = await _schedulerFactory.GetAllSchedulers(cancellationToken);
            foreach (var scheduler in schedulers)
            {
                foreach (var jobGroupName in await scheduler.GetJobGroupNames(cancellationToken))
                {
                    foreach (var triggerKey in await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupEquals(jobGroupName), cancellationToken))
                    {
                        if ((await scheduler.GetTriggerState(triggerKey, cancellationToken)).Equals(TriggerState.Error))
                        {
                            await scheduler.ResumeTrigger(triggerKey, cancellationToken);
                        }
                    }
                }
            }
        }
    }
}