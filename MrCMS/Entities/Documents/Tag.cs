using System.Collections.Generic;
using Iesi.Collections.Generic;

namespace MrCMS.Entities.Documents
{
    public class Tag : SiteEntity
    {
        public Tag()
        {
            Documents = new HashedSet<Document>();
        }
        public virtual string Name { get; set; }

        public virtual Iesi.Collections.Generic.ISet<Document> Documents { get; set; }
    }
}