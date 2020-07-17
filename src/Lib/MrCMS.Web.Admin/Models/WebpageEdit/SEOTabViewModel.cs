using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Admin.Models.WebpageEdit
{
    public class SEOTabViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        [Remote("ValidateUrlIsAllowed", "Webpage", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$",
            ErrorMessage = "Url must alphanumeric characters only with dashes or underscore for spaces.")]
        public string UrlSegment { get; set; }

        [StringLength(250, ErrorMessage = "Meta title cannot be longer than 250 characters.")]
        public string MetaTitle { get; set; }

        [StringLength(250, ErrorMessage = "Meta description cannot be longer than 250 characters.")]
        public string MetaDescription { get; set; }

        [StringLength(250, ErrorMessage = "Meta keywords cannot be longer than 250 characters.")]
        public string MetaKeywords { get; set; }

        public string ExplicitCanonicalLink { get; set; }
        
        [StringLength(250, ErrorMessage = "SEO Target Phrase cannot be longer than 250 characters.")]
        [DisplayName("SEO Target Phrase")]
        public string SEOTargetPhrase { get; set; }

        [DisplayName("Include In Navigation")]
        public bool RevealInNavigation { get; set; }

        public bool IncludeInSitemap { get; set; }

        public bool RequiresSSL { get; set; }

        [StringLength(8000, ErrorMessage = "Custom Header Scripts cannot be longer than 8000 characters.")]
        public string CustomHeaderScripts { get; set; }

        [StringLength(8000, ErrorMessage = "Custom Footer Scripts cannot be longer than 8000 characters.")]
        public string CustomFooterScripts { get; set; }

        public bool DoNotCache { get; set; }
    }
}