using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class LatestXArticles : Widget
    {
        public virtual int NumberOfArticles { get; set; }
        public virtual ArticleList RelatedNewsList { get; set; }
    }
}