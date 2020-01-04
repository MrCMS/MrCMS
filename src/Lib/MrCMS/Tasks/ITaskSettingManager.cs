using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Tasks.Entities;

namespace MrCMS.Tasks
{
    public interface ITaskSettingManager
    {
        Task<IList<TaskInfo>> GetInfo();
        Task StartTasks(List<TaskInfo> scheduledTasks, DateTime startTime);
        Task SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action);
        Task Update(Type type, bool enabled, int frequencyInSeconds);
        Task Reset(Type type, bool resetLastCompleted);
    }
}