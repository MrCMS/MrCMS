using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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