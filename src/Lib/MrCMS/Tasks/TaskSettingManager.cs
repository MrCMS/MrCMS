using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Scheduling;
using MrCMS.Tasks.Entities;
using MrCMS.Website;
using NHibernate;
using NHibernate.Linq;
using Quartz;

namespace MrCMS.Tasks
{
    public class TaskSettingManager : ITaskSettingManager
    {
        private readonly IStatelessSession _session;
        private readonly IQuartzConfigManager _configManager;

        public TaskSettingManager(IStatelessSession session, IQuartzConfigManager configManager)
        {
            _session = session;
            _configManager = configManager;
        }

        public async Task SyncConfig()
        {
            var info = await GetInfo();
            await _configManager.UpdateConfig(info.ToArray());
        }

        public async Task<IList<TaskInfo>> GetInfo()
        {
            var allSettings = await GetAllSettings();
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<SchedulableTask>();

            return types.Select(type =>
            {
                var existingSetting = allSettings.ContainsKey(type) ? allSettings[type] : null;
                return existingSetting == null
                    ? new TaskInfo {Type = type}
                    : new TaskInfo
                    {
                        Type = type,
                        LastCompleted = existingSetting.LastCompleted,
                        LastStarted = existingSetting.LastStarted,
                        Enabled = existingSetting.Enabled,
                        CronSchedule = existingSetting.CronSchedule ?? string.Empty,
                        Status = existingSetting.Status
                    };
            }).ToList();
        }

        public async Task StartTasks(List<TaskInfo> scheduledTasks, DateTime startTime)
        {
            foreach (var scheduledTask in scheduledTasks)
            {
                await Update(scheduledTask.Type, async settings =>
                {
                    settings.Status = TaskExecutionStatus.AwaitingExecution;
                    settings.LastStarted = startTime;
                    await Save(settings);
                });
            }
        }

        public async Task SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action)
        {
            await Update(type, async taskSettings =>
            {
                taskSettings.Status = status;
                action?.Invoke(taskSettings);
                await Save(taskSettings);
            });
        }

        public async Task<bool> Update(Type type, bool enabled, string cronSchedule)
        {
            await CreateIfDoesNotExist(type);
            if (enabled && !CronExpression.IsValidExpression(cronSchedule))
                return false;
            await Update(type, async taskSettings =>
            {
                taskSettings.Enabled = enabled;
                taskSettings.CronSchedule = cronSchedule;
                await Save(taskSettings);
            });
            var info = await GetInfo();
            await _configManager.UpdateConfig(info.First(x => x.Type == type));
            return true;
        }

        private async Task CreateIfDoesNotExist(Type type)
        {
            var allSettings = await GetAllSettings();
            if (allSettings.ContainsKey(type))
                return;
            await _session.InsertAsync(new TaskSettings
                {TypeName = type.FullName, CreatedOn = DateTime.UtcNow, UpdatedOn = DateTime.UtcNow});
        }

        public async Task Reset(Type type, bool resetLastCompleted)
        {
            await Update(type, async taskSettings =>
            {
                if (resetLastCompleted)
                    taskSettings.LastCompleted = null;
                taskSettings.Status = TaskExecutionStatus.Pending;
                await Save(taskSettings);
            });
        }

        private async Task Update(Type type, Func<TaskSettings, Task> action)
        {
            var allSettings = await GetAllSettings();
            if (!allSettings.ContainsKey(type))
                return;

            var taskSettings = allSettings[type];
            await action(taskSettings);
        }

        private async Task Save(TaskSettings taskSettings)
        {
            await _session.TransactAsync(session => session.UpdateAsync(taskSettings));
        }

        private async Task<Dictionary<Type, TaskSettings>> GetAllSettings()
        {
            var listAsync = await _session.Query<TaskSettings>().ToListAsync();
            return listAsync
                .Where(x => x.Type != null)
                .ToDictionary(settings => settings.Type);
        }

        async Task IExecuteOnStartup.Execute(CancellationToken cancellationToken)
        {
            await SyncConfig();
        }

        int IExecuteOnStartup.Order => int.MaxValue;
    }
}