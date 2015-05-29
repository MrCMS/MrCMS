using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports;

namespace MrCMS.Web.Areas.Admin.Services.UserSubscriptionReports
{
    public interface IUserSubscriptionReportsService
    {
        IEnumerable<LineGraphData> GetAllSubscriptions(UserSubscriptionReportsSearchQuery searchQuery);
    }
}