using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MrCMS.Apps
{
    public static class MrCMSAppRegistrationExtensions
    {
        public static MrCMSAppContext AddMrCMSApps(this IServiceCollection serviceCollection,
            Action<MrCMSAppContext> action)
        {
            var appContext = new MrCMSAppContext();

            action(appContext);

            foreach (var app in appContext.Apps)
                serviceCollection = app.RegisterServices(serviceCollection);

            serviceCollection.AddSingleton(appContext);

            return appContext;
        }

        public static IMvcBuilder AddAppMvcConfig(this IMvcBuilder builder, MrCMSAppContext context)
        {
            foreach (var app in context.Apps)
                builder.AddApplicationPart(app.Assembly);

            return builder;
        }

        public static IFileProvider AddFileProvider(this IServiceCollection services,
            IHostingEnvironment environment, MrCMSAppContext appContext)
        {
            var physicalProvider = environment.ContentRootFileProvider;
            var compositeFileProvider =
                new CompositeFileProvider(new[] { physicalProvider }.Concat(appContext.ViewFileProviders));

            services.AddSingleton<IFileProvider>(compositeFileProvider);

            return compositeFileProvider;
        }
    }
}