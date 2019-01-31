using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddMediaCategoryModel
    {
        public int? ParentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlSegment { get; set; }
    }
}