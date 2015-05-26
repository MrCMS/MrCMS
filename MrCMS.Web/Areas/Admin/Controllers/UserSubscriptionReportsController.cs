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
    class UserSubscriptionReportsController : MrCMSAdminController
    {
        private readonly IUserSubscriptionReportsService _userSubscriptionReportsService;

        public UserSubscriptionReportsController(IUserSubscriptionReportsService userSubscriptionReportsService)
        {
            _userSubscriptionReportsService = userSubscriptionReportsService;
        }


        [HttpGet]
        [MrCMSACLRule(typeof(UserSubscriptionReportsACL), UserSubscriptionReportsACL.View)]
        public ActionResult Index()
        {
            UserSubscriptionReportsSearchQuery searchQuery = new UserSubscriptionReportsSearchQuery();
            searchQuery.Subscriptions = new D3UserSubscriptionReportsModel() { JsonObject = _userSubscriptionReportsService.GetAllSubscriptions(searchQuery) };
            return View(searchQuery);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(UserSubscriptionReportsACL), UserSubscriptionReportsACL.Filter)]
        public ActionResult Index(UserSubscriptionReportsSearchQuery searchQuery)
        {
            D3UserSubscriptionReportsModel model = new D3UserSubscriptionReportsModel();
            model.JsonObject = _userSubscriptionReportsService.GetAllSubscriptions(searchQuery);
            searchQuery.Subscriptions = model;
            return View(searchQuery);
        }
    }
}