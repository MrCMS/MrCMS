using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddWebpageModel
    {
        public string DocumentType { get; set; }
        public int? PageTemplateId { get; set; }
        public int? ParentId { get; set; }
        [Required]
        [MaxLength(450)]
        public string Name { get; set; }
        [Required]
        [MaxLength(450)]
        [Remote("ValidateUrlIsAllowed", "Webpage")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$",
            ErrorMessage = "Url must alphanumeric characters only with dashes or underscore for spaces.")]
        public string UrlSegment { get; set; }
    }
}