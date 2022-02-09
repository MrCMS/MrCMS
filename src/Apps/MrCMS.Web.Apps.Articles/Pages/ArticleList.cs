using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class ArticleList : TextPage
    {
        [DisplayName("Page Size"), Range(1, 9999)]
        public virtual int PageSize { get; set; } = 10;

        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }
    }
}
