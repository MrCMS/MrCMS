using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    [WidgetOutputCacheable]
    public class LatestXArticles : Widget
    {
        [Required]
        public virtual int NumberOfArticles { get; set; }

        [Required]
        public virtual ArticleList RelatedNewsList { get; set; }
    }
}
