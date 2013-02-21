using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Application.Models
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

namespace MrCMS.Web.Application.Models
{
    public class NavigationList : List<NavigationRecord>
    {
        public NavigationList(IEnumerable<NavigationRecord> list)
        {
            AddRange(list);
        }
    }
}