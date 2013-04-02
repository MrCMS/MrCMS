using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Articles.Pages
{
    public class Article : TextPage
    {
        [AllowHtml]
        public virtual string Abstract { get; set; }
    }
}