using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web
{
    public class PageTemplate : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }

        [Required]
        [DisplayName("Page Name")]
        public virtual string PageName { get; set; }

        [Required]
        [DisplayName("Page Type")]
        public virtual string PageType { get; set; }

        public virtual Layout.Layout Layout { get; set; }

        [Required]
        [DisplayName("URL Generator Type")]
        public virtual string UrlGeneratorType { get; set; }
    }
}