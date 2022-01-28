using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.ModelBinders;
using MrCMS.Web.Apps.Articles.Models;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Articles.Services;
using MrCMS.Website.Controllers;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Controllers
{
    public class ArticleListController : MrCMSAppUIController<MrCMSArticlesApp>
    {
        private readonly IArticleListService _articleListService;
        private readonly IWebpageUIService _webpageUiService;

        public ArticleListController(IArticleListService articleListService, IWebpageUIService webpageUIService)
        {
            _articleListService = articleListService;
            _webpageUiService = webpageUIService;
        }

        [CanonicalLinks("paged-articles")]
        public async Task<ActionResult> Show(int id,
            [ModelBinder(typeof(ArticleListModelBinder))] ArticleSearchModel model)
        {
            var page = await _webpageUiService.GetPage<ArticleList>(id);
            ViewData["paged-articles"] = await _articleListService.GetArticlesAsync(page, model);
            ViewData["article-search-model"] = model;

            return View(page);
        }
    }
}
