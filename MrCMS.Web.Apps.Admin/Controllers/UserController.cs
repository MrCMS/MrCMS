using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class UserController : MrCMSAdminController
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IUserSearchService _userSearchService;
        private readonly IRoleService _roleService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IGetUserCultureOptions _getUserCultureOptions;

        public UserController(IUserManagementService userManagementService, IUserSearchService userSearchService,
            IRoleService roleService, IPasswordManagementService passwordManagementService,
            IGetUserCultureOptions getUserCultureOptions)
        {
            _userManagementService = userManagementService;
            _userSearchService = userSearchService;
            _roleService = roleService;
            _passwordManagementService = passwordManagementService;
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
        public ActionResult Add(
            [ModelBinder(typeof(AddUserModelBinder))]
            User user) 
        {
            _userManagementService.AddUser(user);

            return RedirectToAction("Edit", new { id = user.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(UserACL), UserACL.Edit)]
        public ActionResult Edit_Get(User user)
        {
            ViewData["AvailableRoles"] = _roleService.GetAllRoles();
            ViewData["OnlyAdmin"] = _roleService.IsOnlyAdmin(user);
            ViewData["culture-options"] = _getUserCultureOptions.Get();

            return user == null
                       ? (ActionResult)RedirectToAction("Index")
                       : View(user);
        }

        [HttpPost]
        [Acl(typeof(UserACL), UserACL.Edit)]
        public ActionResult Edit(
            [ModelBinder(typeof(EditUserModelBinder))]
            User user) 
        {
            _userManagementService.SaveUser(user);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", user.Name));
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
        public RedirectToActionResult Delete(User user)
        {
            _userManagementService.DeleteUser(user);

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
        public ActionResult SetPassword(User user, string password)
        {
            _passwordManagementService.SetPassword(user, password, password);
            _userManagementService.SaveUser(user);
            return RedirectToAction("Edit", new { user.Id });
        }

        public JsonResult IsUniqueEmail(string email, int? id)
        {
            if (_userManagementService.IsUniqueEmail(email, id))
                return Json(true);

            return Json("Email already registered.");
        }
    }
}