using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Tasks
{
    public class ScheduledTask : SiteEntity, IHaveExecutionStatus
    {
        public ScheduledTask()
        {
            EveryXSeconds = 600;
        }

        public virtual string Type { get; set; }

        [DisplayName("Every X Seconds")]
        public virtual int EveryXSeconds { get; set; }

        [DisplayName("Last Queued At")]
        public virtual DateTime? LastQueuedAt { get; set; }

        [DisplayName("Last Run")]
        public virtual DateTime? LastComplete { get; set; }

        [DisplayName("Last Run")]
        public virtual TaskExecutionStatus Status { get; set; }

        public virtual string TypeName
        {
            get { return Type.Split('.').Last().BreakUpString(); }
        }

        public virtual void OnStarting(AdHocTask executableTask)
        {
            Status = TaskExecutionStatus.Executing;
            executableTask.OnStarting();
        }

        public virtual void OnSuccess(AdHocTask executableTask)
        {
            Status = TaskExecutionStatus.Pending;
            LastComplete = CurrentRequestData.Now;
            executableTask.OnSuccess();
        }

        public virtual void OnFailure(AdHocTask executableTask, Exception exception)
        {
            Status = TaskExecutionStatus.Pending;
            executableTask.OnFailure(exception);
        }

        public virtual IEnumerable<SelectListItem> GetTypeOptions()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<SchedulableTask>()
                .Where(type => type.IsPublic)
                .BuildSelectItemList(type => type.Name.BreakUpString(), type => type.FullName,
                    emptyItemText: "Select a type");
        }
    }
}