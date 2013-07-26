using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Entities;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class AuthorInfoController : MrCMSAppAdminController<ArticlesApp>
    {
        private readonly IUserProfileDataService _userProfileDataService;

        public AuthorInfoController(IUserProfileDataService userProfileDataService)
        {
            _userProfileDataService = userProfileDataService;
        }

        [HttpGet]
        public PartialViewResult Add(User user)
        {
            return PartialView(new AuthorInfo { User = user });
        }

        [HttpPost]
        public RedirectToRouteResult Add(AuthorInfo info)
        {
            _userProfileDataService.Add(info);
            return RedirectToAction("Edit", "User", new {id = info.User.Id});
        }

        [HttpGet]
        public PartialViewResult Edit(AuthorInfo info)
        {
            return PartialView(info);
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToRouteResult Edit_POST(AuthorInfo info)
        {
            _userProfileDataService.Update(info);
            return RedirectToAction("Edit", "User", new {id = info.User.Id});
        }

        [ChildActionOnly]
        public PartialViewResult Show(AuthorInfo info)
        {
            return PartialView(info);
        }
    }
}