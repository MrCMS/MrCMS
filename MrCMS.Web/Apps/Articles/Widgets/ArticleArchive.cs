using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    [OutputCacheable(PerPage = true)]
    public class ArticleArchive : Widget
    {
        public virtual ArticleList ArticleList { get; set; }

        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }
    }
}