using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models.WebpageEdit
{
    public class WebpagePropertiesTabViewModel
    {
        public int Id { get; set; }
        public string WebpageType { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string BodyContent { get; set; }
    }
}