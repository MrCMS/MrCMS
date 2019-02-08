namespace MrCMS.Web.Apps.Articles.Models
{
    public class ArticleSearchModel
    {
        public int Page { get; set; } = 1;
        public string Category { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}