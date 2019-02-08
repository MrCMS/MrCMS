using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class UpdateMediaCategoryModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Folder Name")]
        public string Name { get; set; }

        public bool IsGallery { get; set; }
    }
}