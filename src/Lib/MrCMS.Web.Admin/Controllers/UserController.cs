using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Helpers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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
        public RedirectToActionResult Add(AddUserModel addUserModel)
        {
            var addUser = _userAdminService.AddUser(addUserModel);

            return RedirectToAction("Edit", new { id = addUser});
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
        public RedirectToActionResult Edit(UpdateUserModel model, [ModelBinder(typeof(UpdateUserRoleModelBinder))]List<int> roles)
        {
            var user = _userAdminService.SaveUser(model, roles);
            TempData.SuccessMessages().Add($"{user.Name} successfully saved");
            return RedirectToAction("Edit", "User", new { Id = user.Id });
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
        public RedirectToActionResult Delete(int id)
        {
            _userAdminService.DeleteUser(id);

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
        public RedirectToActionResult SetPassword(int id, string password)
        {
            _userAdminService.SetPassword(id, password);
            return RedirectToAction("Edit", new { id });
        }

        public JsonResult IsUniqueEmail(string email, int? id)
        {
            if (_userAdminService.IsUniqueEmail(email, id))
            {
                return Json(true);
            }

            return Json("Email already registered.");
        }
    }
}