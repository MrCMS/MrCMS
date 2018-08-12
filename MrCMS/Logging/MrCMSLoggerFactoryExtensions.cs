using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MrCMS.Logging
{
    public static class MrCMSLoggerFactoryExtensions
    {
        public static ILoggingBuilder AddMrCMSLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, MrCMSDatabaseLoggerProvider>();
            return builder;
        }

        //public static ILoggingBuilder AddFile(this ILoggingBuilder builder>
        //{
        //    builder.AddFile();
        //    builder.Services.Configure(configure);

        //    return builder;
        //}
    }
}