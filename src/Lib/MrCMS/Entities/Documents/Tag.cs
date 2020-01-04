using System.Collections.Generic;

namespace MrCMS.Entities.Documents
{
    public class Tag : SiteEntity
    {
        public Tag()
        {
            DocumentTags = new List<DocumentTag>();
        }

        public virtual string Name { get; set; }

        public virtual IList<DocumentTag> DocumentTags { get; set; }
    }
}