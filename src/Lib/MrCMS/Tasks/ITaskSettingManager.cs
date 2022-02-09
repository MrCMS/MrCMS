using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Tasks.Entities;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public interface ITaskSettingManager : IExecuteOnStartup
    {
        Task SyncConfig();
        Task<IList<TaskInfo>> GetInfo();
        Task StartTasks(List<TaskInfo> scheduledTasks, DateTime startTime);
        Task SetStatus(Type type, TaskExecutionStatus status, Action<TaskSettings> action);
        Task<bool> Update(Type type, bool enabled, string cronSchedule);
        Task Reset(Type type, bool resetLastCompleted);
    }
}