using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class ArticleController : MrCMSAppAdminController<MrCMSArticlesApp>
    {
        private readonly IBelongToUserLookupService _belongToUserLookupService;

        public ArticleController(IBelongToUserLookupService _belongToUserLookupService)
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