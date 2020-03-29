using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UserController : MrCMSAdminController
    {
        private readonly IUserAdminService _userAdminService;
        private readonly IUserSearchService _userSearchService;
        private readonly IRoleService _roleService;
        private readonly IGetUserCultureOptions _getUserCultureOptions;

        public UserController(IUserAdminService userAdminService, IUserSearchService userSearchService,
            IRoleService roleService,
            IGetUserCultureOptions getUserCultureOptions)
        {
            _userAdminService = userAdminService;
            _userSearchService = userSearchService;
            _roleService = roleService;
            _getUserCultureOptions = getUserCultureOptions;
        }

        [Acl(typeof(UserACL), UserACL.View)]
        public ActionResult Index(UserSearchQuery searchQuery)
        {
            ViewData["users"] = _userSearchService.GetUsersPaged(searchQuery);
            ViewData["roles"] = _userSearchService.GetAllRoleOptions();
            return View(searchQuery);
        }

        [HttpGet]
        [Acl(typeof(UserACL), UserACL.Add)]
        public PartialViewResult Add()
        {
            var model = new AddUserModel();
            ViewData["culture-options"] = _getUserCultureOptions.Get();
            return PartialView(model);
        }

        [HttpPost]
        [Acl(typeof(UserACL), UserACL.Add)]
        public async Task<RedirectToActionResult> Add(AddUserModel addUserModel)
        {
            var addUser = await _userAdminService.AddUser(addUserModel);

            return RedirectToAction("Edit", new { id = addUser });
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(UserACL), UserACL.Edit)]
        public ActionResult Edit_Get(User user)
        {
            if (user == null)
                return RedirectToAction("Index");
            ViewData["AvailableRoles"] = _roleService.GetAllRoles();
            ViewData["OnlyAdmin"] = _roleService.IsOnlyAdmin(user);
            ViewData["culture-options"] = _getUserCultureOptions.Get();
            ViewData["user"] = user;

            var updateUserModel = _userAdminService.GetUpdateModel(user);
            return View(updateUserModel);
        }

        [HttpPost]
        [Acl(typeof(UserACL), UserACL.Edit)]
        public async Task<RedirectToActionResult> Edit(UpdateUserModel model, [ModelBinder(typeof(UpdateUserRoleModelBinder))]List<int> roles)
        {
            var user = await _userAdminService.SaveUser(model, roles);
            TempData.SuccessMessages().Add($"{user.Name} successfully saved");
            return RedirectToAction("Edit", "User", new {user.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        [Acl(typeof(UserACL), UserACL.Delete)]
        public PartialViewResult Delete_Get(User user)
        {
            return PartialView(user);
        }

        [HttpPost]
        [Acl(typeof(UserACL), UserACL.Delete)]
        public async Task<RedirectToActionResult> Delete(int id)
        {
            await _userAdminService.DeleteUser(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Acl(typeof(UserACL), UserACL.SetPassword)]
        public ActionResult SetPassword(User user)
        {
            return PartialView(user);
        }

        [HttpPost]
        [Acl(typeof(UserACL), UserACL.SetPassword)]
        public async Task<RedirectToActionResult> SetPassword(int id, string password)
        {
            await _userAdminService.SetPassword(id, password);
            return RedirectToAction("Edit", new { id });
        }

        public async Task<JsonResult> IsUniqueEmail(string email, int? id)
        {
            if (await _userAdminService.IsUniqueEmail(email, id))
            {
                return Json(true);
            }

            return Json("Email already registered.");
        }
    }
}