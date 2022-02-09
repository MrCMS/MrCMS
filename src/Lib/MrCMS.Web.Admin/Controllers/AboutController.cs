using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class AboutController : MrCMSAdminController
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}