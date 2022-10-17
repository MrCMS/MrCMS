using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Tasks;
using Quartz;

namespace MrCMS.Scheduling
{
    public class QuartzConfigManager : IQuartzConfigManager
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzConfigManager> _logger;

        public QuartzConfigManager(ISchedulerFactory schedulerFactory, ILogger<QuartzConfigManager> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task UpdateConfig(params TaskInfo[] info)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            foreach (var taskInfo in info)
            {
                await Update(scheduler, taskInfo, _logger);
            }
        }

        private static async Task Update(IScheduler scheduler, TaskInfo info, ILogger<QuartzConfigManager> logger)
        {
            var isStarted = scheduler.IsStarted;
            var isStandby = scheduler.InStandbyMode;
            logger.LogInformation($"Logger is started {isStarted}. Logger is standby {isStandby}");

            // if the type doesn't exist register it
            var taskType = info.Type;
            var executorType = typeof(ScheduledTaskRunner<>).MakeGenericType(taskType);
            var name = taskType.Name;
            var jobKey = JobKey.Create(name);
            await scheduler.DeleteJob(jobKey);
            var jobDetail = JobBuilder.Create(executorType)
                .WithIdentity(jobKey)
                .StoreDurably()
                .Build();
            await scheduler.AddJob(jobDetail, true);

            var triggerKey = new TriggerKey(name + "-trigger");
            var trigger = await scheduler.GetTrigger(triggerKey);
            // if it's enabled set up a trigger if it doesn't exist
            if (info.Enabled)
            {
                if (trigger == null && CronExpression.IsValidExpression(info.CronSchedule))
                {
                    await scheduler.UnscheduleJob(triggerKey);

                    trigger = TriggerBuilder.Create()
                        .WithIdentity(triggerKey)
                        .WithCronSchedule(info.CronSchedule, builder =>
                        {
                            builder.WithMisfireHandlingInstructionIgnoreMisfires();
                        })
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