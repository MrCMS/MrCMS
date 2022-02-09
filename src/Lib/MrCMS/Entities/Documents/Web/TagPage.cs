using System.Collections.Generic;

namespace MrCMS.Entities.Documents.Web
{
    public class TagPage : Webpage
    {
        public TagPage()
        {
            Webpages = new HashSet<Webpage>();
        }
        public virtual ISet<Webpage> Webpages { get; set; }
    }
}