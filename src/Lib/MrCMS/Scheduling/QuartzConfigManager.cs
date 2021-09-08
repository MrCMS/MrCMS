using System.Threading.Tasks;
using MrCMS.Tasks;
using Quartz;

namespace MrCMS.Scheduling
{
    public class QuartzConfigManager : IQuartzConfigManager
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzConfigManager(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        public async Task UpdateConfig(params TaskInfo[] info)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            foreach (var taskInfo in info)
            {
                await Update(scheduler, taskInfo);
            }
        }

        private static async Task Update(IScheduler scheduler, TaskInfo info)
        {
            // if the type doesn't exist register it
            var taskType = info.Type;
            var executorType = typeof(ScheduledTaskRunner<>).MakeGenericType(taskType);
            var name = taskType.Name;
            var jobKey = JobKey.Create(name);
            var jobDetail = await scheduler.GetJobDetail(jobKey);
            if (jobDetail == null || jobDetail.JobType != executorType)
            {
                jobDetail = JobBuilder.Create(executorType)
                    .WithIdentity(jobKey)
                    .StoreDurably()
                    .Build();
                await scheduler.AddJob(jobDetail, true);
            }

            var triggerKey = new TriggerKey(name + "-trigger");
            var trigger = await scheduler.GetTrigger(triggerKey);
            // if it's enabled set up a trigger if it doesn't exist
            if (info.Enabled)
            {
                if (trigger == null && CronExpression.IsValidExpression(info.CronSchedule))
                {
                    trigger = TriggerBuilder.Create().WithIdentity(triggerKey)
                        .WithCronSchedule(info.CronSchedule)
                        .ForJob(jobKey).Build();

                    await scheduler.ScheduleJob(trigger);
                }
            }
            // else remove it if it does
            else
            {
                if (trigger != null)
                    await scheduler.UnscheduleJob(triggerKey);
            }
        }
    }
}