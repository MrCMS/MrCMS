using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services.UserSubscriptionReports;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports;
using MrCMS.Web.Areas.Admin.ACL.UserSubscriptionReports;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UserSubscriptionReportsController : MrCMSAdminController
    {
        private readonly IUserSubscriptionReportsService _userSubscriptionReportsService;

        public UserSubscriptionReportsController(IUserSubscriptionReportsService userSubscriptionReportsService)
        {
            _userSubscriptionReportsService = userSubscriptionReportsService;
        }

        [MrCMSACLRule(typeof(UserSubscriptionReportsACL), UserSubscriptionReportsACL.View)]
        public ViewResult Index(UserSubscriptionReportsSearchQuery searchQuery)
        {
            var data=_userSubscriptionReportsService.GetAllSubscriptions(searchQuery);
            ViewData["JsonData"] =new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(data);
            return View(searchQuery);
        }
    }
}