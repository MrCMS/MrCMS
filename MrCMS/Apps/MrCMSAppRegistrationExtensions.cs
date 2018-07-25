using System;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Apps
{
    public static class MrCMSAppRegistrationExtensions
    {
        public static MrCMSAppContext AddMrCMSApps(this IServiceCollection serviceCollection,
            Action<MrCMSAppContext> action)
        {
            var sokchoAppContext = new MrCMSAppContext();

            action(sokchoAppContext);

            foreach (var app in sokchoAppContext.Apps)
                serviceCollection = app.RegisterServices(serviceCollection);

            return sokchoAppContext;
        }

        public static IMvcBuilder AddAppMvcConfig(this IMvcBuilder builder, MrCMSAppContext context)
        {
            foreach (var sokchoApp in context.Apps)
                builder.AddApplicationPart(sokchoApp.Assembly);

            return builder;
        }
    }
}