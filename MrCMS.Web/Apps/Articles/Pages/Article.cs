using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Iesi.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class Article : TextPage, IBelongToUser
    {
        public Article()
        {
            OtherSections = new HashedSet<ArticleSection>();
        }

        [AllowHtml]
        [StringLength(160, ErrorMessage = "Abstract cannot be longer than 160 characters.")]
        public virtual string Abstract { get; set; }

        public virtual ISet<ArticleSection> OtherSections { get; set; }

        [DisplayName("Author")]
        public virtual User User { get; set; }
    }
}