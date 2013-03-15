using System.Collections.Generic;

namespace MrCMS.Web.Apps.Core.Models
{
    public class NavigationList : List<NavigationRecord>
    {
        public NavigationList(IEnumerable<NavigationRecord> list)
        {
            AddRange(list);
        }
    }
}