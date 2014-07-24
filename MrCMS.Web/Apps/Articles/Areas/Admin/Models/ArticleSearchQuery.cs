using System.ComponentModel;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class ArticleSearchQuery
    {
        public ArticleSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        [DisplayName("Section")]
        public int? SectionId { get; set; }
    }
}