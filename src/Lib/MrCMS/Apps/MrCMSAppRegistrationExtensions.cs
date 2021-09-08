using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Attributes;
using MrCMS.Services.Resources;
using MrCMS.Website;
using MrCMS.Website.Filters;

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

        public static IMvcBuilder AddMvcForMrCMS(this IServiceCollection services, MrCMSAppContext appContext)
        {
            return services.AddMvc(options =>
                {
                    //options.Filters.Add<EndRequestHandlerFilter>(); //todo is needed with Quartz?
                    options.Filters.Add<GoogleRecaptchaFilter>();
                    options.Filters.Add<ReturnUrlFilter>();
                    options.Filters.Add<CanonicalLinksFilter>();
                    options.Filters.Add<AddWebpageViewsData>();
                    options.Filters.Add<HoneypotFilter>();
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    var index = options.ValueProviderFactories.IndexOf(
                        options.ValueProviderFactories.OfType<QueryStringValueProviderFactory>().Single());
                    options.ValueProviderFactories[index] = new CulturedQueryStringValueProviderFactory();
                    appContext.SetupMvcOptions(options);
                })
                .AddApplicationPart(Assembly.GetAssembly(typeof(MrCMSAppRegistrationExtensions)))
                .AddRazorOptions(options =>
                {
                    options.ViewLocationExpanders.Insert(0, new WebpageViewExpander());
                    options.ViewLocationExpanders.Insert(1, new WidgetViewExpander());
                    options.ViewLocationExpanders.Insert(2, new ThemeViewLocationExpander());
                    options.ViewLocationExpanders.Insert(3, new AppViewLocationExpander());
                })
                .AddMrCMSDataAnnotations()
                .AddDataAnnotationsLocalization()
                .AddAppMvcConfig(appContext);
        }

        public class CulturedQueryStringValueProviderFactory : IValueProviderFactory
        {
            public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                var query = context.ActionContext.HttpContext.Request.Query;
                if (query != null && query.Count > 0)
                {
                    var valueProvider = new QueryStringValueProvider(
                        BindingSource.Query,
                        query,
                        CultureInfo.CurrentCulture);

                    context.ValueProviders.Add(valueProvider);
                }

                return Task.CompletedTask;
            }
        }
    }
}