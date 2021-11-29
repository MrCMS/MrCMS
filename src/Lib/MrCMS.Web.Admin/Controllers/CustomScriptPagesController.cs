using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class CustomScriptPagesController : MrCMSAdminController
    {
        private readonly ICustomScriptPageAdminService _adminService;

        public CustomScriptPagesController(ICustomScriptPageAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<ActionResult> Index(CustomScriptPagesSearchModel searchModel)
        {
            ViewData["results"] = await _adminService.Search(searchModel);

            return View(searchModel);
        }
    } 
}