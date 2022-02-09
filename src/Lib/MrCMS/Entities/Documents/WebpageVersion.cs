using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;

namespace MrCMS.Entities.Documents
{
    public class WebpageVersion : SiteEntity
    {
        public virtual Webpage Webpage { get; set; }
        public virtual User User { get; set; }
        public virtual string Data { get; set; }
    }
}