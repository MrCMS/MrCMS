using System;
using System.Collections.Generic;

namespace MrCMS.Tasks
{
    public interface IScheduledTaskManager
    {
        IEnumerable<ScheduledTask> GetDueTasks();

        BackgroundTask GetTask(ScheduledTask scheduledTask);
        List<ScheduledTask> GetAllTasks();
        void Add(ScheduledTask scheduledTask);
        void Update(ScheduledTask scheduledTask);
        void Delete(ScheduledTask scheduledTask);
    }
}