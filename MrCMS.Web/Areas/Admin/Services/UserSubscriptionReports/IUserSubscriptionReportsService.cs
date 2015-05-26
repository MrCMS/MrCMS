using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports;

namespace MrCMS.Web.Areas.Admin.Services.UserSubscriptionReports
{
    interface IUserSubscriptionReportsService
    {
        IEnumerable<JsonObject> GetAllSubscriptions(UserSubscriptionReportsSearchQuery searchQuery);
    }
}