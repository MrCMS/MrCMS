using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.HealthChecks;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Services.Dashboard
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

        public async Task<HealthCheckResult> CheckType(string typeName)
        {
            var type = TypeHelper.GetTypeByName(typeName);
            if (type == null)
            {
                return new HealthCheckResult();
            }

            var check = _serviceProvider.GetService(type) is IHealthCheck ? await ((IHealthCheck)_serviceProvider.GetService(type)).PerformCheck() : null;
            return check ?? new HealthCheckResult();
        }
    }
}