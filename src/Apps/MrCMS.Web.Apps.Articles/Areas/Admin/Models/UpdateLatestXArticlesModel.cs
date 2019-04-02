using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Articles.Widgets;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class UpdateLatestXArticlesModel : IAddPropertiesViewModel<LatestXArticles>, IUpdatePropertiesViewModel<LatestXArticles>
    {
        [Required]
        public int NumberOfArticles { get; set; } = 5;

        [Required, DisplayName("News List")]
        public int RelatedNewsListId { get; set; }
    }
}