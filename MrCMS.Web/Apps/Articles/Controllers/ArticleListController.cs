using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Controllers
{
    public class ArticleListController : MrCMSAppUIController<ArticlesApp>
    {
        public ActionResult View(ArticleList page)
        {
            var category = Request["category"];
            var pageVal = Request["p"];
            int pageNum;
            if (!int.TryParse(pageVal, out pageNum))
            {
                pageNum = 1;
            }

            return View("~/Apps/Articles/Views/Pages/ArticleList.cshtml", page.CurrentLayout.UrlSegment, new CategoryContainer<Pages.Article>(page, category, pageNum));
        }
    }
}