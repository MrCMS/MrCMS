using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web
{
    public class PageTemplate : SiteEntity
    {
        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual string PageTemplateName { get; set; }

        [Required]
        public virtual string PageType { get; set; }

        public virtual Layout.Layout Layout { get; set; }
        public int? LayoutId { get; set; }

        [Required]
        public virtual string UrlGeneratorType { get; set; }

        public virtual bool SingleUse { get; set; }
    }
}