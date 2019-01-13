using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Web
{
    public class UrlHistory : SiteEntity
    {
        [Required]
        public virtual string UrlSegment { get; set; }

        public virtual Webpage Webpage { get; set; }
    }
}