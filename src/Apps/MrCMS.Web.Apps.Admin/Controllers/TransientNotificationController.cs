using Microsoft.AspNetCore.Mvc;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class TransientNotificationController : MrCMSAdminController
    {
        public PartialViewResult Show()
        {
            return PartialView();
        }
    }
}