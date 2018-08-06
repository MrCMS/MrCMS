using System;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class SEOTabViewModel
    {
        public int Id { get; set; }
        public string UrlSegment { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string ExplicitCanonicalLink { get; set; }
        public string SEOTargetPhrase { get; set; }
        public bool RevealInNavigation { get; set; }
        public bool IncludeInSitemap { get; set; }
        public bool RequiresSSL { get; set; }
        public string CustomHeaderScripts { get; set; }
        public string CustomFooterScripts { get; set; }
        public bool DoNotCache { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public DateTime UpdatedOn { get; set; }
    }
}