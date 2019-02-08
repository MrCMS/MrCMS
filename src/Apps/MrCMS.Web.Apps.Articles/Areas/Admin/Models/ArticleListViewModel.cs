using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class ArticleListViewModel : IAddPropertiesViewModel<ArticleList>, IUpdatePropertiesViewModel<ArticleList> 
    {
        [DisplayName("Page Size")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Page size must be a number")]
        [Range(1, 9999)]
        public int PageSize { get; set; }

        [DisplayName("Allow Paging")]
        public bool AllowPaging { get; set; }

        [DisplayName("Tags")]
        public string TagList { get; set; }
    }
}
