using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class ArticleController :MrCMSAppAdminController<ArticlesApp>
    {
        private readonly IUserService _userService;

        public ArticleController(IUserService userService)
        {
            _userService = userService;
        }

        [ChildActionOnly]
        public PartialViewResult ForUser(User user)
        {
            var articles = _userService.GetAll<Article>(user);
            return PartialView(articles);
        }
    }
}