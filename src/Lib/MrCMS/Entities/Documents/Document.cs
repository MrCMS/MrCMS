using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents
{
    public abstract class Document : SiteEntity
    {
        protected Document()
        {
            Versions = new List<DocumentVersion>();
            Tags = new HashSet<Tag>();
        }

        [Required] [StringLength(255)] public virtual string Name { get; set; }

        public virtual Document Parent { get; set; }

        [Required]
        [DisplayName("Display Order")]
        public virtual int DisplayOrder { get; set; }

        public virtual string UrlSegment { get; set; }

        public virtual ISet<Tag> Tags { get; set; }

        public virtual string TagList
        {
            get { return string.Join(",", Tags.Select(x => x.Name)); }
        }

        public virtual int ParentId => Parent?.Id ?? 0;

        public virtual string DocumentType => GetType().Name;

        protected internal virtual IList<DocumentVersion> Versions { get; set; }

        public virtual bool HideInAdminNav { get; set; }

        public virtual ISet<TagPage> TagPages { get; set; }

        public virtual string TagPageList
        {
            get { return string.Join(",", TagPages?.Select(x => x.Name) ?? new List<string>()); }
        }
    }
}