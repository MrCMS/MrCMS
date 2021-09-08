using System.Collections.Generic;
using MrCMS.Web.Admin.Models.UserSubscriptionReports;

namespace MrCMS.Web.Admin.Services.UserSubscriptionReports
{
    public interface IUserRegistrationsReportsService
    {
        IEnumerable<UserRegistrationsGraphModel> GetRegistrations(UserRegistrationReportsSearchQuery searchQuery);
    }
    
    public class UserRegistrationsGraphModel
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }
}