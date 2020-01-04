using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Helpers;
using MrCMS.Tasks.Entities;

namespace MrCMS.Tasks
{
    public class TaskSettingManager : ITaskSettingManager
    {
        private readonly IGlobalRepository<TaskSettings> _repository;

        public TaskSettingManager(IGlobalRepository<TaskSettings> repository)
        {
            _repository = repository;
        }

        public async Task<IList<TaskInfo>> GetInfo()
        {
            var allSettings = await GetAllSettings();
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<SchedulableTask>();

            return types.Select(type =>
            {
                var existingSetting = allSettings.ContainsKey(type) ? allSettings[type] : null;
                return existingSetting == null
                    ? new TaskInfo { Type = type }
                    : new TaskInfo
                    {
                        Type = type,
                        LastCompleted = existingSetting.LastCompleted,
                        LastStarted = existingSetting.LastStarted,
                        Enabled = existingSetting.Enabled,
                        FrequencyInSeconds = existingSetting.FrequencyInSeconds,
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

        public async Task Update(Type type, bool enabled, int frequencyInSeconds)
        {
            await CreateIfDoesNotExist(type);
            await Update(type, async taskSettings =>
            {
                taskSettings.Enabled = enabled;
                taskSettings.FrequencyInSeconds = frequencyInSeconds;
                await Save(taskSettings);
            });
        }

        private async Task CreateIfDoesNotExist(Type type)
        {
            var allSettings = await GetAllSettings();
            if (allSettings.ContainsKey(type))
                return;
            await _repository.Add(new TaskSettings { TypeName = type.FullName });
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
            await _repository.Update(taskSettings);
        }

        private async Task<Dictionary<Type, TaskSettings>> GetAllSettings()
        {
            var list = await _repository.Query().ToListAsync();
            return list.Where(x => x.Type != null)
                .ToDictionary(settings => settings.Type);
        }
    }
}