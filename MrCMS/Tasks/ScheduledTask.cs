using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Tasks
{
    public class ScheduledTask : SiteEntity
    {
        public virtual string Type { get; set; }

        [DisplayName("Every X Minutes")]
        public virtual int EveryXMinutes { get; set; }

        [DisplayName("Last Run")]
        public virtual DateTime? LastRun { get; set; }

        public virtual IEnumerable<SelectListItem> GetTypeOptions()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<BackgroundTask>()
                             .Where(type => type.IsPublic)
                             .BuildSelectItemList(type => type.Name, type => type.FullName,
                                                  emptyItemText: "Select a type");
        }
    }
}