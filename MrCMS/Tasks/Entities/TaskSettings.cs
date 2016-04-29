using System;
using MrCMS.Entities;
using MrCMS.Helpers;

namespace MrCMS.Tasks.Entities
{
    public class TaskSettings : SystemEntity
    {
        public TaskSettings()
        {
            FrequencyInSeconds = 60;
        }
        public virtual bool Enabled { get; set; }
        public virtual int FrequencyInSeconds { get; set; }
        public virtual DateTime? LastStarted { get; set; }
        public virtual DateTime? LastCompleted { get; set; }
        public virtual TaskExecutionStatus Status { get; set; }
        public virtual string TypeName { get; set; }

        public virtual Type Type
        {
            get { return TypeHelper.GetTypeByName(TypeName); }
        }
    }
}