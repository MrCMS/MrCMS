using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class SEOTabViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(450)]
        [Remote("ValidateUrlIsAllowed", "Webpage")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$",
            ErrorMessage = "Url must alphanumeric characters only with dashes or underscore for spaces.")]
        public string UrlSegment { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string ExplicitCanonicalLink { get; set; }
        
        [StringLength(250, ErrorMessage = "SEO Target Phrase cannot be longer than 250 characters.")]
        [DisplayName("SEO Target Phrase")]
        public string SEOTargetPhrase { get; set; }
        public bool RevealInNavigation { get; set; }
        public bool IncludeInSitemap { get; set; }
        public bool RequiresSSL { get; set; }
        public string CustomHeaderScripts { get; set; }
        public string CustomFooterScripts { get; set; }
        public bool DoNotCache { get; set; }
    }
}