using System;
using System.Collections.Generic;
using MrCMS.Tasks.Entities;

namespace MrCMS.Tasks
{
    public interface ITaskSettingManager
    {
        IList<TaskInfo> GetInfo();
        void StartTasks(List<TaskInfo> scheduledTasks, DateTime startTime);
        void SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action);
        void Update(Type type, bool enabled, int frequencyInSeconds);
        void Reset(Type type, bool resetLastCompleted);
    }
}