using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents
{
    public class Tag : SiteEntity
    {
        public Tag()
        {
            Webpages = new HashSet<Webpage>();
        }

        public virtual string Name { get; set; }

        public virtual ISet<Webpage> Webpages { get; set; }
    }
}