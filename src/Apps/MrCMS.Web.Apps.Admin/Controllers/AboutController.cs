using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class AboutController : MrCMSAdminController
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}