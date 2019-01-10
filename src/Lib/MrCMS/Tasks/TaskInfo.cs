using System;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class TaskInfo
    {
        public bool Enabled { get; set; }
        public int FrequencyInSeconds { get; set; } = 60;
        public DateTime? LastStarted { get; set; }
        public DateTime? LastCompleted { get; set; }
        public TaskExecutionStatus Status { get; set; }
        public Type Type { get; set; }

        public string TypeName
        {
            get { return Type.FullName; }
        }

        public string Name
        {
            get { return Type.Name.BreakUpString(); }
        }
    }
}