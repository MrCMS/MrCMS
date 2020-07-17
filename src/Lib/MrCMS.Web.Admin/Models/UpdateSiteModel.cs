using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Web.Admin.Models
{
    public class UpdateSiteModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Base URL")]
        [Required]
        [RegularExpression(@"^(?i)(?!http).*$", ErrorMessage = "Url should be without HTTP")]
        public string BaseUrl { get; set; }

        public string StagingUrl { get; set; }
    }
}