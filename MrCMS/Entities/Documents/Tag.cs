using System.Collections.Generic;

namespace MrCMS.Entities.Documents
{
    public class Tag : SiteEntity
    {
        public Tag()
        {
            Documents = new HashSet<Document>();
        }

        public virtual string Name { get; set; }

        public virtual ISet<Document> Documents { get; set; }
    }
}