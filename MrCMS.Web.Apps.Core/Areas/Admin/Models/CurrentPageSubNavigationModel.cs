using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class CurrentPageSubNavigationModel : IUpdatePropertiesViewModel<CurrentPageSubNavigation>,
        IAddPropertiesViewModel<CurrentPageSubNavigation>
    {
        public bool ShowNameAsTitle { get; set; }
    }
}