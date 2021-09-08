using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;

namespace MrCMS.Website
{
    public class StartupService : IHostedService
    {
        private readonly ILogger<StartupService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public StartupService(ILogger<StartupService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var executeOnStartupTypes =
                TypeHelper.GetAllConcreteTypesAssignableFrom<IExecuteOnStartup>();
            using var scope = _serviceProvider.CreateScope();
            var executeOnStartups = executeOnStartupTypes.Select(x => scope.ServiceProvider.GetService(x))
                .OfType<IExecuteOnStartup>();
            foreach (var onStartup in executeOnStartups.OrderBy(x => x.Order))
            {
                var name = onStartup.GetType().FullName;
                _logger.LogInformation("Starting Task: '{0}'", name);
                await onStartup.Execute(cancellationToken);
                _logger.LogInformation("Completed Task: '{0}'", name);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}