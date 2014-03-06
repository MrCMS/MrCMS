using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Apps.Core.Models.Navigation
{
    public class NavigationRecord
    {
        public NavigationRecord()
        {
            Children = new List<NavigationRecord>();
        }
        public MvcHtmlString Text { get; set; }

        public MvcHtmlString Url { get; set; }

        public List<NavigationRecord> Children { get; set; }
    }
}