using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class ArticleList : TextPage
    {
        [DisplayName("Page Size")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Page size must be a number")]
        [Range(1, 9999)]
        public virtual int PageSize { get; set; }

        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }
    }
}