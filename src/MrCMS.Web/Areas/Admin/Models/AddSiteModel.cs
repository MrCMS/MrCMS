using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class AddSiteModel
    {
        [Required]
        public string Name { get; set; }

        [DisplayName("Base URL")]
        [Required]
        [RegularExpression(@"^(?i)(?!http).*$", ErrorMessage = "Url should be without HTTP")]
        public string BaseUrl { get; set; }
    }
}