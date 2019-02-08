using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MrCMS.Web.Apps.Admin.Infrastructure.ModelBinding;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class ArticleViewModel : IUpdatePropertiesViewModel<Article>, 
        IAddPropertiesViewModel<Article>
    {
        [StringLength(500, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public string Abstract { get; set; }

        [DisplayName("Author")]
        public int? UserId { get; set; }

        [DisplayName("Featured Image")]
        public string FeatureImage { get; set; }

        [DisplayName("Tags")]
        public string TagList { get; set; }
    }
}