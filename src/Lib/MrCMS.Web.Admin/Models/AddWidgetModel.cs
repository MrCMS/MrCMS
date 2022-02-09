using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class AddWidgetModel 
    {
        public int LayoutAreaId { get; set; }
        [Required]
        public string WidgetType { get; set; }

        public string Name { get; set; }
        public bool ForPage { get; set; }
    }
}