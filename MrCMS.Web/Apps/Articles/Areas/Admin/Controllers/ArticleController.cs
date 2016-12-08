using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class ArticleController : MrCMSAppAdminController<ArticlesApp>
    {
        private readonly IBelongToUserLookupService _belongToUserLookupService;

        public ArticleController(IBelongToUserLookupService belongToUserLookupService)
        {
            _belongToUserLookupService = belongToUserLookupService;
        }

        [ChildActionOnly]
        public PartialViewResult ForUser(User user)
        {
            var articles = _belongToUserLookupService.GetAll<Article>(user);
            return PartialView(articles);
        }
    }
}