using System;
using System.Collections.Generic;
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
using MrCMS.Web.Admin;
using MrCMS.Web.Apps.Core;
using MrCMS.Web.Apps.Core.Auth;
using MrCMS.Website;
using MrCMS.Website.Caching;
using MrCMS.Website.CMS;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Hosting;
using MrCMS.Settings;
using MrCMS.Web.Apps.Articles;
using StackExchange.Profiling.Storage;
using ISession = NHibernate.ISession;

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
            var isInstalled = IsInstalled();
            services.AddRequiredServices();
            services.Configure<SystemConfig>(Configuration.GetSection("SystemConfig"));
            services.AddCultureInfo(Configuration);
            
            var appContext = services.AddMrCMSApps(context =>
            {
                context.RegisterApp<MrCMSAdmin>();
                context.RegisterApp<MrCMSCoreApp>();
                context.RegisterApp<MrCMSArticlesApp>();
                context.RegisterTheme<RedTheme>();
                context.RegisterDatabaseProvider<SqliteProvider>();
            });

            services.AddMrCMSData(isInstalled, Configuration);            
            services.AddSiteProvider();
            services.AddMrCMSFileSystem();
            services.AddSignalR();
            services.AddAutoMapper(expression =>
            {
                expression.AllowNullDestinationValues = true;
                appContext.ConfigureAutomapper(expression);
            }, GetType().Assembly);

            if (!isInstalled)
            {
                services.AddInstallationServices();
                return;
            }

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
                return new ClearableInMemoryCache(
                    new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(cacheOptions)));
            });
            services.AddSingleton<IMemoryCache>(provider => provider.GetRequiredService<IClearableInMemoryCache>());
            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
            services.AddMiniProfiler();

            services.AddScoped(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.AddScoped(provider => provider.GetService<IStringLocalizerFactory>()
                .Create(null, null));

            var authenticationBuilder = services.AddAuthentication();
            services.AddExternalAuthProviders(authenticationBuilder);
            
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IPostConfigureOptions<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache
                >());
            services
                .AddSingleton<IOptionsMonitorCache<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache
                >();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", builder => builder.RequireRole(UserRole.Administrator));
            });
        }

        private bool IsInstalled()
        {
            var dbSection = Configuration.GetConnectionString("mrcms");
            return dbSection?.Length > 0;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MrCMSAppContext appContext)
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
            
            app.UseMrCMS(builder =>
            {
                app.UseRequestLocalization();
                builder.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new CompositeFileProvider(
                        new[] {Environment.WebRootFileProvider}.Concat(appContext.ContentFileProviders))
                });
                builder.UseAuthentication();
                builder.UseMiniProfiler();
            });
        }
    }
}