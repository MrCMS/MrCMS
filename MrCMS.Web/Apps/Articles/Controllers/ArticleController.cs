using System.Web.Mvc;
using MrCMS.Attributes;
using MrCMS.Web.Apps.Articles.ModelBinders;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Controllers
{
    public class ArticleController : MrCMSAppUIController<ArticlesApp>
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [CanonicalLinks("paged-articles")]
        public ActionResult Show(ArticleList page, [IoCModelBinder(typeof(ArticleListModelBinder))]ArticleSearchModel model)
        {
            ViewData["paged-articles"] = _articleService.GetArticles(page, model);
            ViewData["article-search-model"] = model;
            return View(page);
        }
    }
}