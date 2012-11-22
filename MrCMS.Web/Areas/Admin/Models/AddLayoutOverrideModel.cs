using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddLayoutAreaOverrideModel : LayoutAreaOverride
    {
        public string WidgetType { get; set; }
        public IEnumerable<SelectListItem> WidgetTypes { get; set; }
    }
}