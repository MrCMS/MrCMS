using System;
using System.Collections.Generic;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services.Events;
using MrCMS.Services.Events.Args;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Criterion;
using System.Linq;
using MrCMS.Web.Areas.Admin.Helpers.D3Reports;
using MrCMS.Web.Areas.Admin.Models.UserSubscriptionReports;

namespace MrCMS.Web.Areas.Admin.Services.UserSubscriptionReports
{
    public class UserSubscriptionReportsService : IUserSubscriptionReportsService
    {
        private readonly ISession _session;

        public UserSubscriptionReportsService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<JsonObject> GetAllSubscriptions(UserSubscriptionReportsSearchQuery searchQuery)
        {
            return _session.Query<User>().Where(c => c.CreatedOn >= Convert.ToDateTime(searchQuery.StartDate) && c.CreatedOn <= Convert.ToDateTime(searchQuery.EndDate)).AsEnumerable()
                           .GroupBy(c => c.CreatedOn.ToString("MMM") + " " + c.CreatedOn.Year)
                           .Select(c => new JsonObject { JoiningMonthYear = c.Key, Count = c.Count() });
        }
    }
}
