using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class ArticleControllerController : MrCMSAppAdminController<MrCMSArticlesApp>
    {
        private readonly IBelongToUserLookupService _belongToUserLookupService;

        public ArticleControllerController(IBelongToUserLookupService _belongToUserLookupService)
        {
            this._belongToUserLookupService = _belongToUserLookupService;
        }

        public PartialViewResult ForUser(User user)
        {
            var articles = _belongToUserLookupService.GetAll<Article>(user);
            return PartialView(articles);
        }
    }
}