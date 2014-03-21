using System.Web.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class TransientNotificationController : MrCMSAdminController
    {
        public PartialViewResult Show()
        {
            return PartialView();
        }
    }
}