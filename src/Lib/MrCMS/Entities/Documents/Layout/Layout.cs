using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Layout
{
    public class Layout : SiteEntity
    {
        public Layout()
        {
            LayoutAreas = new List<LayoutArea>();
        }
        [Required] [StringLength(255)] public virtual string Name { get; set; }
        public virtual Layout Parent { get; set; }
        [Required]
        [DisplayName("Display Order")]
        public virtual int DisplayOrder { get; set; }

        public virtual string Path { get; set; }
        public virtual bool HideInAdminNav { get; set; }

        public virtual IList<LayoutArea> LayoutAreas { get; set; }

        public virtual IList<PageTemplate> PageTemplates { get; set; }

        public virtual bool Hidden { get; set; }
    }
}