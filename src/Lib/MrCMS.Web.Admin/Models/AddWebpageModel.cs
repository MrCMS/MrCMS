using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Admin.Models
{
    public class AddWebpageModel
    {
        public string WebpageType { get; set; }
        public int? PageTemplateId { get; set; }
        public int? ParentId { get; set; }
        [Required]
        [MaxLength(450)]
        [DisplayName("Page Name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(450)]
        [Remote("ValidateUrlIsAllowed", "Webpage")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$",
            ErrorMessage = "Url must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Url")]
        public string UrlSegment { get; set; }
    }
}