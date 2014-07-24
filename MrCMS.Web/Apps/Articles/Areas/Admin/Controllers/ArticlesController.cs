using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class ArticlesController : MrCMSAppAdminController<ArticlesApp>
    {
        private readonly IArticleAdminService _articleAdminService;

        public ArticlesController(IArticleAdminService articleAdminService)
        {
            _articleAdminService = articleAdminService;
        }

        public ViewResult Index(ArticleSearchQuery query)
        {
            ViewData["article-section-options"] = _articleAdminService.GetArticleSectionOptions();
            ViewData["primary-section-options"] = _articleAdminService.GetPrimarySectionOptions();
            ViewData["results"] = _articleAdminService.Search(query);
            return View(query);
        }
    }
}