using System.Collections.Generic;

namespace MrCMS.Web.Apps.Core.Models.Navigation
{
    public class NavigationList : List<NavigationRecord>
    {
        public NavigationList(IEnumerable<NavigationRecord> list)
        {
            AddRange(list);
        }
        
        public static readonly NavigationList Empty = new NavigationList(new List<NavigationRecord>());
    }
}