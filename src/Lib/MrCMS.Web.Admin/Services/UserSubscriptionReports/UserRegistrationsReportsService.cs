using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models.UserSubscriptionReports;
using NHibernate;

namespace MrCMS.Web.Admin.Services.UserSubscriptionReports
{
    public class UserRegistrationsReportsService : IUserRegistrationsReportsService
    {
        private readonly ISession _session;

        public UserRegistrationsReportsService(ISession session)
        {
            _session = session;
        }

        public IEnumerable<UserRegistrationsGraphModel> GetRegistrations(UserRegistrationReportsSearchQuery searchQuery)
        {
            var query = from user in _session.Query<User>()
                where user.CreatedOn >= searchQuery.StartDate && user.CreatedOn <= searchQuery.EndDate
                group user by new {user.CreatedOn.Year, user.CreatedOn.Month, user.CreatedOn.Day}
                into userGroup
                orderby userGroup.Key.Year, userGroup.Key.Month, userGroup.Key.Day
                select new
                {
                    userGroup.Key.Day,
                    userGroup.Key.Month,
                    userGroup.Key.Year,
                    Count = userGroup.Count()
                };

            return query.Select(arg => new UserRegistrationsGraphModel
            {
                Date = new DateTime(arg.Year, arg.Month, arg.Day).ToShortDateString(),
                Count = arg.Count
            }).ToList();
        }
    }
}