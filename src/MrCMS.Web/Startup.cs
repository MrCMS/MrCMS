using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MrCMS.Apps;
using MrCMS.Data.Sqlite;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Globalization;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Themes.Red;
using MrCMS.Web.Apps.Admin;
using MrCMS.Web.Apps.Core;
using MrCMS.Web.Apps.Core.Auth;
using MrCMS.Website;
using MrCMS.Website.Caching;
using MrCMS.Website.CMS;
using System.Globalization;
using System.Linq;
using ISession = NHibernate.ISession;

namespace MrCMS.Web
{
    public class Startup
    {
        private const string Database = nameof(Database);

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            TypeHelper.Initialize(GetType().Assembly);
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var isInstalled = IsInstalled();

            // services always required
            services.RegisterAllSimplePairings();
            services.RegisterOpenGenerics();
            services.SelfRegisterAllConcreteTypes();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //todo: something else with this, just making sure the library is loaded for now
            services.AddTransient<CreateSQLiteDatabase>();

            var supportedCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new UserProfileRequestCultureProvider());
            });

            var appContext = services.AddMrCMSApps(context =>
            {
                context.RegisterApp<MrCMSCoreApp>();
                context.RegisterApp<MrCMSAdminApp>();
                context.RegisterTheme<RedTheme>();
            });

            services.AddMrCMSDataAccess(isInstalled, Configuration.GetSection(Database));

            // TODO: Look to removing Site for constructors and resolving like this
            services.AddScoped(provider =>
            {
                var site = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-site"] as Site;

                site = site ?? provider.GetRequiredService<ICurrentSiteLocator>().GetCurrentSite();
                var session = provider.GetRequiredService<ISession>();
                if (site != null)
                {
                    session.EnableFilter("SiteFilter").SetParameter("site", site.Id);
                }
                return site;
            });

            services.AddMrCMSFileSystem();

            services.AddSignalR();

            services.AddAutoMapper(expression =>
            {
                expression.AllowNullDestinationValues = true;
                appContext.ConfigureAutomapper(expression);
            });


            // if the system is not installed we just want MrCMS to show the installation screen
            if (!isInstalled)
            {
                services.AddInstallationServices();
                return;
            }

            // Live services

            var fileProvider = services.AddFileProvider(Environment, appContext);

            services.RegisterSettings();
            services.RegisterFormRenderers();
            services.RegisterTokenProviders();
            services.RegisterTasks();

            services.AddMvcForMrCMS(appContext, fileProvider);

            services.AddLogging(builder => builder.AddMrCMSLogger());

            services.AddMrCMSIdentity(Configuration);

            services.AddSingleton<ICmsMethodTester, CmsMethodTester>();
            services.AddSingleton<IGetMrCMSParts, GetMrCMSParts>();
            services.AddSingleton<IAssignPageDataToRouteData, AssignPageDataToRouteData>();
            services.AddSingleton<IQuerySerializer, QuerySerializer>();

            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IClearableInMemoryCache>(provider =>
            {
                var cacheOptions = new MemoryCacheOptions();
                return new ClearableInMemoryCache(new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(cacheOptions)));
            });
            services.AddSingleton<IMemoryCache>(provider => provider.GetRequiredService<IClearableInMemoryCache>());



            services.AddScoped(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.AddScoped(provider => provider.GetService<IStringLocalizerFactory>()
                .Create(null, null));


            services.AddAuthentication();
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IPostConfigureOptions<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache>());
            services
                .AddSingleton<IOptionsMonitorCache<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", builder => builder.RequireRole(UserRole.Administrator));
            });
        }

        private bool IsInstalled()
        {
            var dbSection = Configuration.GetSection(Database);
            return dbSection.Exists();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MrCMSAppContext appContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            if (!IsInstalled())
            {
                app.ShowInstallation();
                return;
            }

            app.UseRequestLocalization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new CompositeFileProvider(
                    new[] { Environment.WebRootFileProvider }.Concat(appContext.ContentFileProviders))
            });
            app.UseAuthentication();
            app.UseMrCMS();
        }
    }
}