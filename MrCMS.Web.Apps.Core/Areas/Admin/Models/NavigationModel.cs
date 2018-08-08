using MrCMS.Web.Apps.Admin.Models.Tabs;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models
{
    public class NavigationModel : IUpdatePropertiesViewModel<Navigation>, IAddPropertiesViewModel<Navigation>
    {
        public bool IncludeChildren { get; set; }
    }
}