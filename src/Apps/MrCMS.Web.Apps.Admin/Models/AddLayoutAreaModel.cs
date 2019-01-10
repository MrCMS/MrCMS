using System.ComponentModel.DataAnnotations;
using OfficeOpenXml.Utils;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddLayoutAreaModel
    {
        public int LayoutId { get; set; }
        
        [Required]
        public string AreaName { get; set; }
    }
}