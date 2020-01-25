using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MrCMS.Logging
{
    public static class MrCMSLoggerFactoryExtensions
    {
        public static ILoggingBuilder AddMrCMSLogger(this ILoggingBuilder builder)
        {
            //builder.Services.AddSingleton<ILoggerProvider, MrCMSDatabaseLoggerProvider>();
            return builder;
        }
    }
}