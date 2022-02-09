using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class TransientNotificationController : MrCMSAdminController
    {
        public PartialViewResult Show()
        {
            return PartialView();
        }
    }
}