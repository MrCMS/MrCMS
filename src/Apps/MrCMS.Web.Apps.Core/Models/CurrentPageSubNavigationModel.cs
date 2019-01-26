using System.Collections.Generic;
using MrCMS.Web.Apps.Core.Models.Navigation;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Models
{
    public class CurrentPageSubNavigationModel
    {
        public List<NavigationRecord> NavigationList { get; set; }
        public CurrentPageSubNavigation Model { get; set; }
    }
}