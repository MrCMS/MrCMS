using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents
{
    public abstract class ContentBlock : SiteEntity, ISortable
    {
        [Required, StringLength(200)]
        public virtual string Name { get; set; }
        public virtual Webpage Webpage { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual string DisplayName => Name;
    }

}