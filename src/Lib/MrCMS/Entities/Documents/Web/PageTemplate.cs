using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web
{
    public class PageTemplate : SiteEntity
    {
        public PageTemplate()
        {
            Webpages = new List<Webpage>();
        }

        [Required] public virtual string Name { get; set; }

        [Required] public virtual string PageTemplateName { get; set; }

        [Required] public virtual string PageType { get; set; }

        public virtual Layout.Layout Layout { get; set; }

        [Required] public virtual string UrlGeneratorType { get; set; }

        public virtual bool SingleUse { get; set; }

        public virtual IList<Webpage> Webpages { get; set; }
    }
}