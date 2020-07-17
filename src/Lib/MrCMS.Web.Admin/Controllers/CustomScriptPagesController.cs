using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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