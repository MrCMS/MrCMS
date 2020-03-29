using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddLayoutAreaModel
    {
        public int LayoutId { get; set; }
        
        [Required]
        [StringLength(250, ErrorMessage = "Area name max length 250")]
        public string AreaName { get; set; }
    }
}