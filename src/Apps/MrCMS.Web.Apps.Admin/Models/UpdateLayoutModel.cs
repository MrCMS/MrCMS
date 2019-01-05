using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class UpdateLayoutModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlSegment { get; set; }
        public bool Hidden { get; set; }
    }

    public class UpdateMediaCategoryModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsGallery { get; set; }
    }
    public class AddMediaCategoryModel
    {
        public int? ParentId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlSegment { get; set; }
    }
}