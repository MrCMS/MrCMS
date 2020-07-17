using MrCMS.HealthChecks;
using MrCMS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Admin.Services.Dashboard
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IServiceProvider _serviceProvider;

        public HealthCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<IHealthCheck> GetHealthChecks()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<IHealthCheck>();
            return types.Select(type => _serviceProvider.GetService(type)).OfType<IHealthCheck>().ToList();
        }

        public HealthCheckResult CheckType(string typeName)
        {
            var type = TypeHelper.GetTypeByName(typeName);
            if (type == null)
            {
                return new HealthCheckResult();
            }

            return (_serviceProvider.GetService(type) as IHealthCheck)?.PerformCheck() ?? new HealthCheckResult();
        }
    }
}