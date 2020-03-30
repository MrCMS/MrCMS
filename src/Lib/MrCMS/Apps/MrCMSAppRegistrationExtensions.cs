using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Services.Resources;
using MrCMS.Website;
using MrCMS.Website.Filters;
using MrCMS.Website.Profiling;
using StackExchange.Profiling.Internal;

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

        private static void MakeFiltersInstallationAware(this MvcOptions options)
        {
            var copy = options.Filters.ToList();
            options.Filters.Clear();

            foreach (var metadata in copy)
            {
                // if it's a type we'll wrap it
                if (metadata is TypeFilterAttribute factory)
                {
                    var filterType = factory.ImplementationType;
                    if (filterType.IsImplementationOf(typeof(IActionFilter)))
                        options.Filters.Add(typeof(InstallationAwareActionFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IAsyncActionFilter)))
                        options.Filters.Add(typeof(InstallationAwareAsyncActionFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IAuthorizationFilter)))
                        options.Filters.Add(typeof(InstallationAwareAuthorizationFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IAsyncAuthorizationFilter)))
                        options.Filters.Add(typeof(InstallationAwareAsyncAuthorizationFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IResourceFilter)))
                        options.Filters.Add(typeof(InstallationAwareResourceFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IAsyncResourceFilter)))
                        options.Filters.Add(typeof(InstallationAwareAsyncResourceFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IResultFilter)))
                        options.Filters.Add(typeof(InstallationAwareResultFilter<>).MakeGenericType(filterType));
                    else if (filterType.IsImplementationOf(typeof(IAsyncResultFilter)))
                        options.Filters.Add(typeof(InstallationAwareAsyncResultFilter<>).MakeGenericType(filterType));
                    else
                    {
                        throw new Exception($"Cannot make filter '{filterType.FullName}' installation aware");
                    }
                }
                else
                {
                    // otherwise we'll just keep it as-is
                    options.Filters.Add(metadata);
                }
            }
        }

        public static IMvcBuilder AddMvcForMrCMS(this IServiceCollection services, MrCMSAppContext appContext,
            IFileProvider viewFileProvider, Action<MvcOptions> additionalConfig)
        {
            return services.AddMvc(options =>
                {
                    // add custom binder to beginning of collection
                    options.ModelBinderProviders.Insert(0, new SystemEntityModelBinderProvider());
                    options.Filters.Add<EndRequestHandlerFilter>();
                    options.Filters.Add<ProfilingAsyncActionFilter<WebpageCachingFilter>>();
                    options.Filters.Add<ActionProfilingFilter>();
                    options.Filters.Add<ResultProfilingFilter>();
                    options.Filters.Add<GoogleRecaptchaFilter>();
                    options.Filters.Add<HoneypotFilter>();
                    options.Filters.Add<DoNotCacheFilter>();
                    options.Filters.Add<ProfilingActionFilter>();
                    options.EnableEndpointRouting = false;                    
                    appContext.SetupMvcOptions(options);
                    options.MakeFiltersInstallationAware();
                    additionalConfig(options);
                })
                .AddApplicationPart(Assembly.GetAssembly(typeof(MrCMSAppRegistrationExtensions)))
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(viewFileProvider);
                })
                .AddRazorOptions(options =>
                {
                    options.ViewLocationExpanders.Insert(0, new WebpageViewExpander());
                    options.ViewLocationExpanders.Insert(1, new AppViewLocationExpander());
                    options.ViewLocationExpanders.Insert(2, new ThemeViewLocationExpander());
                })
                .AddViewLocalization()
                .AddMrCMSDataAnnotations()
                .AddDataAnnotationsLocalization()
                .AddAppMvcConfig(appContext);
        }

        public static IFileProvider AddViewFileProvider(this IServiceCollection services,
            IWebHostEnvironment environment, MrCMSAppContext appContext)
        {
            var physicalProvider = environment.ContentRootFileProvider;
            var compositeFileProvider =
                new CompositeFileProvider(new[] { physicalProvider, new InstallationViewFileProvider() }.Concat(appContext.ViewFileProviders));

            services.AddSingleton<IFileProvider>(compositeFileProvider);

            return compositeFileProvider;
        }
    }

}