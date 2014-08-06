using System.ComponentModel;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleArchive : Widget
    {
        public virtual ArticleList ArticleList { get; set; }

        [DisplayName("Show Name As Title")]
        public virtual bool ShowNameAsTitle { get; set; }
    }
}