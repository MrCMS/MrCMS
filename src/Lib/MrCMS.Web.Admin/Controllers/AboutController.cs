using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;

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