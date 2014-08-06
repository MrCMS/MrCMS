using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class Article : TextPage, IBelongToUser
    {
        [AllowHtml]
        [StringLength(160, ErrorMessage = "Abstract cannot be longer than 500 characters.")]
        public virtual string Abstract { get; set; }

        [DisplayName("Author")]
        public virtual User User { get; set; }
    }
}