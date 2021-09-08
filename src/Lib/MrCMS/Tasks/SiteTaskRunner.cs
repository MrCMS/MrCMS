using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;

namespace MrCMS.Tasks
{
    public class SiteTaskRunner : ISiteTaskRunner
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SiteTaskRunner> _logger;

        public SiteTaskRunner(IServiceProvider serviceProvider, ILogger<SiteTaskRunner> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        public async Task ExecuteTask(string typeName)
        {
            var type = TypeHelper.GetTypeByName(typeName);

            if (type!=null && _serviceProvider.GetService(type) is SiteTask task)
            {
                _logger.LogInformation($"Executing '{type.FullName}'");
                await task.Execute();
                _logger.LogInformation($"Executed '{type.FullName}'");
            }
            else
            {
                _logger.LogInformation($"Could not resolve '{typeName}'");
            }
        }
    }
}