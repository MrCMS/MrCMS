using System.Collections.Generic;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.UserSubscriptionReports;

namespace MrCMS.Web.Apps.Admin.Services.UserSubscriptionReports
{
    public interface IUserSubscriptionReportsService
    {
        IEnumerable<LineGraphData> GetAllSubscriptions(UserSubscriptionReportsSearchQuery searchQuery);
    }
}