using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets
{
    public class CurrentPageSubNavigationModel : IUpdatePropertiesViewModel<CurrentPageSubNavigation>,
        IAddPropertiesViewModel<CurrentPageSubNavigation>
    {
        public bool ShowNameAsTitle { get; set; }
    }
}