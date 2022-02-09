using System;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Models
{
    public class QuickSearchResult
    {
        public int id { get; set; }

        public string value { get; set; }

        public string actionUrl => $"/Admin/{entityType}/{action}/{id}";
        private string action => type == typeof(MediaCategory) ? "Show" : "Edit";
        private Type type => TypeHelper.GetTypeByName(systemType);

        public string systemType { get; set; }

        public string displayType
        {
            get
            {
                return type == null ? "" : type.Name.BreakUpString();
            }
        }

        public string systemId
        {
            get { return id.ToString(); }
        }

        public string entityType { get; set; }
    }
}