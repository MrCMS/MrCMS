using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports;
using NHibernate;
using NHibernate.Linq;

namespace MrCMS.Web.Areas.Admin.Services.UserSubscriptionReports
{
    public class UserSubscriptionReportsService : IUserSubscriptionReportsService
    {
        private readonly ISession _session;

        public UserSubscriptionReportsService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<LineGraphData> GetAllSubscriptions(UserSubscriptionReportsSearchQuery searchQuery)
        {
            var query = from user in _session.Query<User>()
                where user.CreatedOn >= searchQuery.StartDate && user.CreatedOn <= searchQuery.EndDate
                group user by new {user.CreatedOn.Year, user.CreatedOn.Month}
                into userGroup
                orderby userGroup.Key.Year, userGroup.Key.Month
                select new
                {
                    userGroup.Key.Month,
                    userGroup.Key.Year,
                    Count = userGroup.Count()
                };

            return query.Select(arg => new LineGraphData
            {
                x = new DateTime(arg.Year, arg.Month, 1).ToString("MMM yyyy"),
                y = arg.Count
            }).ToList();
        }
    }
}