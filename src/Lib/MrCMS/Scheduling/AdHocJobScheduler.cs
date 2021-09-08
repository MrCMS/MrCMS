using System;
using System.Threading.Tasks;
using MrCMS.Tasks;
using Quartz;

namespace MrCMS.Scheduling
{
    public class AdHocJobScheduler : IAdHocJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public AdHocJobScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task Schedule<TJob>() where TJob : SchedulableTask
        {
            var type = typeof(TJob);
            await Schedule(type);
        }

        public async Task Schedule(Type type)
        {
            if (type == null)
                return;
            var scheduler = await _schedulerFactory.GetScheduler();
            var name = type.Name;
            var jobKey = JobKey.Create(name);
            await scheduler.TriggerJob(jobKey, new JobDataMap());
            if (await scheduler.CheckExists(jobKey))
            {
                await scheduler.TriggerJob(jobKey);
            }
        }
    }
}