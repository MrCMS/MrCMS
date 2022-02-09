using System;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskInfo
    {
        public bool Enabled { get; set; }
        // public int FrequencyInSeconds { get; set; } = 60;
        public string CronSchedule { get; set; }
        public DateTime? LastStarted { get; set; }
        public DateTime? LastCompleted { get; set; }
        public TaskExecutionStatus Status { get; set; }
        public Type Type { get; set; }

        public string TypeName => Type.FullName;

        public string Name => Type.Name.BreakUpString();
    }
}