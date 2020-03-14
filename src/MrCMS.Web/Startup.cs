using System;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Settings;
using MrCMS.Web.Apps.Articles;
using StackExchange.Profiling.Storage;

namespace MrCMS.Web
{
    public class Startup
    {
        private const string Database = nameof(Database);

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            TypeHelper.Initialize(GetType().Assembly);
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //var isInstalled = IsInstalled();
            var allMrCmsAssemblies = TypeHelper.GetAllMrCMSAssemblies();
            allMrCmsAssemblies.Add(typeof(SqliteProvider).Assembly);

            // services always required
            services.RegisterAllSimplePairings();
            services.RegisterOpenGenerics();
            services.SelfRegisterAllConcreteTypes();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICheckInstallationStatus, CheckInstallationStatus>();

            var reflectionHelper = new ReflectionHelper(allMrCmsAssemblies.ToArray());
            services.AddSingleton<IReflectionHelper>(reflectionHelper);
            services.AddSingleton(reflectionHelper);

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
                context.RegisterApp<MrCMSArticlesApp>();
                context.RegisterTheme<RedTheme>();
                //context.RegisterDatabaseProvider<SqliteProvider>();
            });

            // TODO: Look to removing Site for constructors and resolving like this
            services.AddScoped(async provider =>
            {
                var site =
                    provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-site"] as Site;

                site ??= await provider.GetRequiredService<ICurrentSiteLocator>().GetCurrentSite();

                return site;
            });

            services.AddMrCMSFileSystem();

            services.AddSignalR();

            var currentAssembly = GetType().Assembly;
            services.AddAutoMapper(expression =>
            {
                expression.AllowNullDestinationValues = true;
                appContext.ConfigureAutomapper(expression);
            }, currentAssembly);


            // if the system is not installed we just want MrCMS to show the installation screen
            //if (!isInstalled)
            //{
            //    services.AddInstallationServices();
            //    return;
            //}

            // Live services

            var fileProvider = services.AddViewFileProvider(Environment, appContext);

            services.AddMrCMSData(reflectionHelper, Configuration, currentAssembly);
            services.RegisterCurrentSite();
            //services.RegisterSettings();
            services.RegisterFormRenderers();
            services.RegisterTokenProviders();
            services.RegisterTasks();
            services.RegisterEvents();

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
                return new ClearableInMemoryCache(
                    new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(cacheOptions)));
            });
            services.AddSingleton<IMemoryCache>(provider => provider.GetRequiredService<IClearableInMemoryCache>());
            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
            services.AddMiniProfiler(options =>
            {
                // All of this is optional. You can simply call .AddMiniProfiler() for all defaults

                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) Control storage
                // (default is 30 minutes in MemoryCacheStorage)
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);

                // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();

                // (Optional) To control authorization, you can use the Func<HttpRequest, bool> options:
                // (default is everyone can access profilers)
                options.ResultsAuthorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;
                options.ResultsListAuthorize = MiniProfilerAuth.IsUserAllowedToSeeMiniProfilerUI;

                // (Optional)  To control which requests are profiled, use the Func<HttpRequest, bool> option:
                // (default is everything should be profiled)
                options.ShouldProfile = MiniProfilerAuth.ShouldStartFor;

                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;
            });

            services.AddScoped(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.AddScoped(provider => provider.GetService<IStringLocalizerFactory>()
                .Create(null, null));

            var authenticationBuilder = services.AddAuthentication();
            //var serviceProvider = services.BuildServiceProvider();
            var thirdPartyAuthSettings = Configuration.GetSection("ThirdPartyAuthSettings").Get<ThirdPartyAuthSettings>() ?? new ThirdPartyAuthSettings();

            if (thirdPartyAuthSettings.GoogleEnabled)
            {
                authenticationBuilder.AddGoogle(options =>
                {
                    options.ClientId = thirdPartyAuthSettings.GoogleClientId;
                    options.ClientSecret = thirdPartyAuthSettings.GoogleClientSecret;
                });
            }

            if (thirdPartyAuthSettings.FacebookEnabled)
            {
                authenticationBuilder.AddFacebook(options =>
                {
                    options.AppId = thirdPartyAuthSettings.FacebookAppId;
                    options.AppSecret = thirdPartyAuthSettings.FacebookAppSecret;
                    options.Fields.Add("email");
                    options.Scope.Add("email");
                });
            }

            //services.TryAddEnumerable(ServiceDescriptor
            //    .Transient<IPostConfigureOptions<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache
            //    >());
            //services
            //    .AddSingleton<IOptionsMonitorCache<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache
            //    >();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", builder => builder.RequireRole(Role.Administrator));
            });
        }

        //private bool IsInstalled()
        //{
        //    var dbSection = Configuration.GetSection(Database);
        //    return dbSection.Exists();
        //}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MrCMSAppContext appContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            using var scope = app.ApplicationServices.CreateScope();
            var checkStatus = scope.ServiceProvider.GetRequiredService<ICheckInstallationStatus>();
            var status = checkStatus.GetStatus();

            if (status == InstallationStatus.RequiresDatabaseSettings || status == InstallationStatus.RequiresMigrations)
            {
                app.ShowInstallation(status);
                return;
            }

            var context = scope.ServiceProvider.GetRequiredService<IMrCmsContextResolver>().Resolve();
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();

            app.MapWhen(
                httpContext => httpContext.RequestServices.GetRequiredService<ICheckInstallationStatus>().IsInstalled(),
                builder =>
                {
                    builder.UseMrCMS(a =>
                    {
                        a.UseRequestLocalization();
                        a.UseStaticFiles(new StaticFileOptions
                        {
                            FileProvider = new CompositeFileProvider(
                                new[] { Environment.WebRootFileProvider }.Concat(appContext.ContentFileProviders))
                        });
                        a.UseAuthentication();
                        a.UseMiniProfiler();
                    });
                });
            app.MapWhen(
                httpContext =>
                    !httpContext.RequestServices.GetRequiredService<ICheckInstallationStatus>().IsInstalled(),
                builder =>
                {
                    if (status == InstallationStatus.RequiresInstallation)
                    {
                        app.ShowInstallation(status);
                        return;
                    }
                });


        }
    }
}