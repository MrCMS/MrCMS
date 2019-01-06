using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddLayoutModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlSegment { get; set; }
        public int? ParentId { get; set; }
    }
}