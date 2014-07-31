using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Iesi.Collections.Generic;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class ArticleSection : TextPage
    {
        public ArticleSection()
        {
            ArticlesInOtherSections = new HashedSet<Article>();
        }

        [DisplayName("Page Size")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Page size must be a number")]
        [Range(1,9999)]
        public virtual int PageSize { get; set; }

        [DisplayName("Allow Paging")]
        public virtual bool AllowPaging { get; set; }

        public virtual ISet<Article> ArticlesInOtherSections { get; set; }
    }
}