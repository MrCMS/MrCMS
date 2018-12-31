using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class WebpagePropertiesTabViewModel
    {
        public int Id { get; set; }
        public string LiveUrlSegment { get; set; }
        public string DocumentType { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string BodyContent { get; set; }
    }
}