using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UserController : AdminController
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
            return View(_userService.GetAllUsers());
        }

        [HttpGet]
        public ActionResult Add()
        {
            var model = new AddUserModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Add([SessionModelBinder(typeof (AddUserModelBinder))] User user)
        {
            _userService.SaveUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            User user = _userService.GetUser(id);

            ViewData["AvailableRoles"] = _roleService.GetAllRoles();

            if (user == null)
                return RedirectToAction("Index");

            return View(user);
        }

        [HttpPost]
        public ActionResult Edit([SessionModelBinder(typeof (EditUserModelBinder))] User user)
        {
            _userService.SaveUser(user);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SetPassword(int id)
        {
            return PartialView(id);
        }

        [HttpPost]
        public ActionResult SetPassword(User user, string password)
        {
            _authorisationService.SetPassword(user, password, password);
            return RedirectToAction("Edit", new {user.Id});
        }
    }
}