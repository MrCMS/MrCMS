using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddLayoutModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Layout File Name")]
        public string UrlSegment { get; set; }

        public int? ParentId { get; set; }
    }
}