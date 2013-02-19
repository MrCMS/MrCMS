using System.Collections.Generic;

namespace MrCMS.Web.Application.Models
{
    public class NavigationRecord
    {
        public NavigationRecord()
        {
            Children = new List<NavigationRecord>();
        }
        public string Text { get; set; }

        public string Url { get; set; }

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