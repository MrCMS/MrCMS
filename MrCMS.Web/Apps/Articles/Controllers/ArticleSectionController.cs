using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.ModelBinders;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Controllers
{
    public class ArticleSectionController : MrCMSAppUIController<ArticlesApp>
    {
        private readonly IArticleService _articleService;

        public ArticleSectionController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public ActionResult Show(ArticleSection page,
            [IoCModelBinder(typeof (ArticleSectionModelBinder))] ArticleSearchModel model)
        {
            ViewData["paged-articles"] = _articleService.GetArticles(page, model);
            ViewData["article-search-model"] = model;
            if (model.Page > 1)
            {
                SetPageTitle(page.Name + ": page " + model.Page);
            }
            return View(page); 
        }
    }
}