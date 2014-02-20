using System.Collections.Generic;
using System.Linq;
using MrCMS.HealthChecks;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Services.Dashboard
{
    public interface IHealthCheckService
    {
        List<IWebsiteHealthCheck> GetHealthChecks();
    }

    public class HealthCheckService : IHealthCheckService
    {
        private readonly IEnumerable<IWebsiteHealthCheck> _websiteHealthChecks;

        public HealthCheckService(IEnumerable<IWebsiteHealthCheck> websiteHealthChecks)
        {
            _websiteHealthChecks = websiteHealthChecks;
        }

        public List<IWebsiteHealthCheck> GetHealthChecks()
        {
            return _websiteHealthChecks.ToList();
        }
    }
}