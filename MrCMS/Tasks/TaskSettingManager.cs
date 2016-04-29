using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Tasks.Entities;
using NHibernate;

namespace MrCMS.Tasks
{
    public class TaskSettingManager : ITaskSettingManager
    {
        private readonly ISession _session;

        public TaskSettingManager(ISession session)
        {
            _session = session;
        }

        public IList<TaskInfo> GetInfo()
        {
            var allSettings = GetAllSettings();
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

        public void StartTasks(List<TaskInfo> scheduledTasks, DateTime startTime)
        {
            foreach (var scheduledTask in scheduledTasks)
            {
                Update(scheduledTask.Type, settings =>
                {
                    settings.Status = TaskExecutionStatus.AwaitingExecution;
                    settings.LastStarted = startTime;
                });
            }
        }

        public void SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action)
        {
            Update(type, taskSettings =>
            {
                taskSettings.Status = status;
                if (action != null)
                    action(taskSettings);
            });
        }

        public void Update(Type type, bool enabled, int frequencyInSeconds)
        {
            CreateIfDoesNotExist(type);
            Update(type, taskSettings =>
            {
                taskSettings.Enabled = enabled;
                taskSettings.FrequencyInSeconds = frequencyInSeconds;
            });
        }

        private void CreateIfDoesNotExist(Type type)
        {
            if (GetAllSettings().ContainsKey(type))
                return;
            _session.Transact(session =>
            {
                session.Save(new TaskSettings {TypeName = type.FullName});
            });
        }

        public void Reset(Type type, bool resetLastCompleted)
        {
            Update(type, taskSettings =>
            {
                if (resetLastCompleted)
                    taskSettings.LastCompleted = null;
                taskSettings.Status = TaskExecutionStatus.Pending;
            });
        }

        private void Update(Type type, Action<TaskSettings> action)
        {
            var allSettings = GetAllSettings();
            if (!allSettings.ContainsKey(type))
                return;

            var taskSettings = allSettings[type];
            action(taskSettings);
            Save(taskSettings);
        }

        private void Save(TaskSettings taskSettings)
        {
            _session.Transact(session => session.Update(taskSettings));
        }

        private Dictionary<Type, TaskSettings> GetAllSettings()
        {
            return _session.QueryOver<TaskSettings>().Cacheable().List().ToDictionary(settings => settings.Type);
        }
    }
}