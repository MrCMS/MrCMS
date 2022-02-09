using System.ComponentModel.DataAnnotations;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    [WidgetOutputCacheable(PerPage = true)]
    public class ArticleCategories : Widget
    {
        [Required]
        public virtual ArticleList ArticleList { get; set; }
        public virtual bool ShowNameAsTitle { get; set; }
        public virtual string Category => ""; //CurrentRequestData.CurrentContext.Request["category"];
    }
}