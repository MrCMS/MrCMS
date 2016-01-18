using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Tasks;

namespace MrCMS.Settings
{
    //public class TaskSettings : SystemSettingsBase
    //{
    //    public TaskSettings()
    //    {
    //        EnabledStatuses = new Dictionary<string, bool>();
    //        Frequencies = new Dictionary<string, int>();
    //        LastStarted = new Dictionary<string, DateTime?>();
    //        LastCompleted = new Dictionary<string, DateTime?>();
    //        TaskExecutionStatuses = new Dictionary<string, TaskExecutionStatus>();
    //    }

    //    public Dictionary<string, bool> EnabledStatuses { get; set; }
    //    public Dictionary<string, int> Frequencies { get; set; }
    //    public Dictionary<string, DateTime?> LastStarted { get; set; }
    //    public Dictionary<string, DateTime?> LastCompleted { get; set; }
    //    public Dictionary<string, TaskExecutionStatus> TaskExecutionStatuses { get; set; }

    //    public IEnumerable<TaskInfo> GetTaskInfo()
    //    {
    //        var types = TypeHelper.GetAllConcreteTypesAssignableFrom<SchedulableTask>();

    //        DateTime? date;
    //        bool boolValue;
    //        int intValue;
    //        TaskExecutionStatus status;
    //        return types.Select(type => new TaskInfo
    //        {
    //            Type = type,
    //            LastCompleted = LastCompleted.TryGetValue(type.FullName, out date) ? date : null,
    //            LastStarted = LastStarted.TryGetValue(type.FullName, out date) ? date : null,
    //            Enabled = EnabledStatuses.TryGetValue(type.FullName, out boolValue) && boolValue,
    //            FrequencyInSeconds = Frequencies.TryGetValue(type.FullName, out intValue) ? intValue : 60,
    //            Status =
    //                TaskExecutionStatuses.TryGetValue(type.FullName, out status) ? status : TaskExecutionStatus.Pending
    //        }).ToList();
    //    }

    //    public class TaskInfo
    //    {
    //        public bool Enabled { get; set; }
    //        public int FrequencyInSeconds { get; set; } = 60;
    //        public DateTime? LastStarted { get; set; }
    //        public DateTime? LastCompleted { get; set; }
    //        public TaskExecutionStatus Status { get; set; }
    //        public Type Type { get; set; }

    //        public string TypeName
    //        {
    //            get { return Type.FullName; }
    //        }

    //        public string Name
    //        {
    //            get { return Type.Name.BreakUpString(); }
    //        }
    //    }
    //}
}