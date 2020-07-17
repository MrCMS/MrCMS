using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Models;
using MrCMS.Web.Admin.Infrastructure.Services;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Entities;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Controllers
{
    public class AuthorInfoController : MrCMSAppAdminController<MrCMSArticlesApp>
    {
        private readonly IUserProfileAdminService _adminService;

        public AuthorInfoController(IUserProfileAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet]
        public PartialViewResult Add(int id)
        {
            return PartialView(new AddAuthorInfoModel { UserId = id });
        }

        [HttpPost]
        public RedirectToActionResult Add(AddAuthorInfoModel info)
        {
            _adminService.Add<AuthorInfo, AddAuthorInfoModel>(info);
            return RedirectToAction("Edit", "User", new { id = info.UserId });
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            return PartialView(_adminService.GetEditModel<AuthorInfo, EditAuthorInfoModel>(id));
        }

        [HttpPost]
        [ActionName("Edit")]
        public RedirectToActionResult Edit_POST(EditAuthorInfoModel model)
        {
            var info = _adminService.Update<AuthorInfo, EditAuthorInfoModel>(model);
            return RedirectToAction("Edit", "User", new { id = info?.User.Id });
        }

        //public PartialViewResult Show(User user)
        //{
        //    return PartialView(user);
        //}
    }

    public class EditAuthorInfoModel : IHaveId
    {
        int? IHaveId.Id => Id;
        public int Id { get; set; }
        public string Bio { get; set; }
    }
}
