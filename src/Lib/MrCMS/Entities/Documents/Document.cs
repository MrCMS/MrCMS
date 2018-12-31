using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MrCMS.Entities.Documents
{
    public abstract class Document : SiteEntity
    {
        protected Document()
        {
            Versions = new List<DocumentVersion>();
            Tags = new HashSet<Tag>();
        }

        [Required]
        [StringLength(255)]
        public virtual string Name { get; set; }

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

        public virtual int ParentId
        {
            get { return Parent == null ? 0 : Parent.Id; }
        }

        public virtual string DocumentType
        {
            get { return GetType().Name; }
        }

        protected internal virtual IList<DocumentVersion> Versions { get; set; }

        public virtual bool HideInAdminNav { get; set; }
    }
}