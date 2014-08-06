using MrCMS.DbConfiguration.Mapping;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Models
{
    public class ArticleSearchModel
    {
        public ArticleSearchModel()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public string Category { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}