using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UserController : MrCMSAdminController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAuthorisationService _authorisationService;

        public UserController(IUserService userService, IRoleService roleService, IAuthorisationService authorisationService)
        {
            _userService = userService;
            _roleService = roleService;
            _authorisationService = authorisationService;
        }

        public ActionResult Index(int page = 1)
        {
            return View(_userService.GetAllUsersPaged(page));
        }

        [HttpGet]
        public PartialViewResult Add()
        {
            var model = new AddUserModel();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Add([IoCModelBinder(typeof (AddUserModelBinder))] User user)
        {
            _userService.AddUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public ActionResult Edit_Get(User user)
        {
            ViewData["AvailableRoles"] = _roleService.GetAllRoles();
            ViewData["OnlyAdmin"] = _roleService.IsOnlyAdmin(user);

            return user == null
                       ? (ActionResult) RedirectToAction("Index")
                       : View(user);
        }

        [HttpPost]
        public ActionResult Edit([IoCModelBinder(typeof (EditUserModelBinder))] User user)
        {
            _userService.SaveUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public PartialViewResult Delete_Get(User user)
        {
            return PartialView(user);
        }

        [HttpPost]
        public RedirectToRouteResult Delete(User user)
        {
            _userService.DeleteUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SetPassword(User user)
        {
            return PartialView(user);
        }

        [HttpPost]
        public ActionResult SetPassword(User user, string password)
        {
            _authorisationService.SetPassword(user, password, password);
            _userService.SaveUser(user);
            return RedirectToAction("Edit", new {user.Id});
        }
    }
}