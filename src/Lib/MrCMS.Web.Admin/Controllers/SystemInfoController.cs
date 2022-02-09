using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class SystemInfoController : MrCMSAdminController
    {
        private readonly IGetSystemInfo _getSystemInfo;

        public SystemInfoController(IGetSystemInfo getSystemInfo)
        {
            _getSystemInfo = getSystemInfo;
        }
        // GET
        public IActionResult Index()
        {
            return View(_getSystemInfo.Get());
        }
    }
}