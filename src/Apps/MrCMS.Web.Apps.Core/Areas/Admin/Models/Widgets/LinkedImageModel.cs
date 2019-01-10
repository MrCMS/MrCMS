using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets
{
    public class LinkedImageModel : IUpdatePropertiesViewModel<LinkedImage>, IAddPropertiesViewModel<LinkedImage>
    {
        public string Image { get; set; }
        public string Link { get; set; }
    }
}