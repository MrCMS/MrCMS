using MrCMS.Web.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets
{
    public class NavigationModel : IUpdatePropertiesViewModel<Navigation>, IAddPropertiesViewModel<Navigation>
    {
        public bool IncludeChildren { get; set; }
    }
}