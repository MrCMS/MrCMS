using System;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class TaskUpdateData
    {
        public string Name { get; set; }
        public string TypeName { get; set; }

        public Type Type
        {
            get { return TypeHelper.GetTypeByName(TypeName); }
        }

        public bool Enabled { get; set; }
        public int FrequencyInSeconds { get; set; }
    }
}