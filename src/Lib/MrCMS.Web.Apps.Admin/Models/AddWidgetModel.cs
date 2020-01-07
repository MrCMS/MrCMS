using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddWidgetModel 
    {
        public int LayoutAreaId { get; set; }
        public int? WebpageId { get; set; }
        public bool IsRecursive { get; set; }
        [Required]
        public string WidgetType { get; set; }

        public string Name { get; set; }
        public bool ForPage { get; set; }
    }
}