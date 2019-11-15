using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MrCMS.Web.Apps.Articles;
using MrCMS.Web.Apps.WebApi;
using MrCMS.Web.Apps.WebApi.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage;
using StackExchange.Profiling.Storage;
using ISession = NHibernate.ISession;
using MrCMS.Web.Apps.IdentityServer4.Admin;
using MrCMS.Web.Apps.WebAPIExample;

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
            //services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
                context.RegisterDatabaseProvider<SqliteProvider>();
                 context.RegisterApp<MrCmsWebApiApp>();
                 context.RegisterApp<MrCMSIs4App>();
                context.RegisterApp<MrCMSIs4AdminApp>();
                context.RegisterApp<MrCmsWebApiSampleApp>();
            });

            services.AddMrCMSDataAccess(isInstalled, Configuration.GetSection(Database));

            // TODO: Look to removing Site for constructors and resolving like this
            services.AddScoped(provider =>
            {
                var site =
                    provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-site"] as Site;

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


            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie()
             .AddJwtBearer("Bearer", options =>
              {
                  options.Authority = "http://localhost:7000";
                  options.RequireHttpsMetadata = false;

                  options.Audience = "api1";
                  // options.SaveToken = true;
              })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "http://localhost:7000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "mvc";

                options.SaveTokens = true;
            });
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IPostConfigureOptions<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache
                >());
            services
                .AddSingleton<IOptionsMonitorCache<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache
                >();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", builder =>
                {
                    builder.RequireRole(UserRole.Administrator);
                    builder.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                });
            });

            
        }

        private bool IsInstalled()
        {
            var dbSection = Configuration.GetSection(Database);
            return dbSection.Exists();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MrCMSAppContext appContext, IApiVersionDescriptionProvider provider)
        {
            app.UseMvc();

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

            app.UseMrCMSSwagger(provider);
          
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

           
            //app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //// specifying the Swagger JSON endpoint.
            //app.UseSwaggerUI(options =>
            //{
            //    // options.SwaggerEndpoint("/swagger/v1/swagger.json", "Mr CMS API V1");
            //    foreach (var description in provider.ApiVersionDescriptions)
            //    {
            //        options.SwaggerEndpoint(
            //            $"/swagger/{description.GroupName}/swagger.json",
            //            description.GroupName.ToUpperInvariant());
            //    }
            //});
        }
    }
}