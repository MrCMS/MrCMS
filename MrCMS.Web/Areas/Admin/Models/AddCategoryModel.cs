using System.Collections.Generic;
using System.Web.Mvc;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddCategoryModel : Controller
    {
        public string Title { get; set; }
        public string BodyContent { get; set; }
        public int? ParentCategoryId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}