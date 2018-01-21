using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class CustomScriptPagesController : MrCMSAdminController
    {
        private readonly ICustomScriptPageAdminService _adminService;

        public CustomScriptPagesController(ICustomScriptPageAdminService adminService)
        {
            _adminService = adminService;
        }

        public ActionResult Index(CustomScriptPagesSearchModel searchModel)
        {
            ViewData["results"] = _adminService.Search(searchModel);

            return View(searchModel);
        }
    }
}