using System.Collections.Generic;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.UserSubscriptionReports;

namespace MrCMS.Web.Admin.Services.UserSubscriptionReports
{
    public interface IUserSubscriptionReportsService
    {
        IEnumerable<LineGraphData> GetAllSubscriptions(UserSubscriptionReportsSearchQuery searchQuery);
    }
}