using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.ACL.UserSubscriptionReports;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models.UserSubscriptionReports;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Controllers
{
    public class UserRegistrationReportsController : MrCMSAdminController
    {
        [Acl(typeof(UserRegistrationReportsACL), UserRegistrationReportsACL.View)]
        public ViewResult Index(UserRegistrationReportsSearchQuery searchQuery)
        {
            return View(searchQuery);
        }
    }
}