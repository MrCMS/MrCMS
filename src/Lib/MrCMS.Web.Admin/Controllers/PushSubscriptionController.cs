using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class PushSubscriptionController : MrCMSAdminController
    {
        private readonly IPushSubscriptionAdminService _adminService;

        public PushSubscriptionController(IPushSubscriptionAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<ViewResult> Index(PushSubscriptionSearchQuery searchQuery)
        {
            ViewData["results"] = await _adminService.Search(searchQuery);
            return View(searchQuery);
        }
    }
}