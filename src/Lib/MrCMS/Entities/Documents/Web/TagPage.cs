using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web
{
    public class TagPage : Webpage
    {
        public TagPage()
        {
            Documents = new HashSet<Document>();
        }
        public virtual ISet<Document> Documents { get; set; }
    }
}