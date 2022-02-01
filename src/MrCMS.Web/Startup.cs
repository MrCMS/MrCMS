using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Themes.Red;
using MrCMS.Web.Admin;
using MrCMS.Web.Apps.Core;
using MrCMS.Website;
using MrCMS.Website.CMS;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MrCMS.Logging;
using MrCMS.Scheduling;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Articles;
using NHibernate;
using Quartz;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;

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
            if (Environment.IsProduction())
            {
                var connectionString = Configuration["DataProtectionConnectionString"];
                var keyName = Configuration["DataProtectionKeyName"];
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    services
                        .AddDataProtection()
                        .SetApplicationName("FleetNews")
                        .PersistKeysToAzureBlobStorage(
                            connectionString,
                            "datakeys", keyName);
                }
            }

            var isInstalled = IsInstalled();
            if (isInstalled)
            {
                services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    q.UsePersistentStore(options =>
                    {
                        options.UseSqlServer(providerOptions =>
                        {
                            providerOptions.ConnectionString = Configuration.GetConnectionString("mrcms");
                        });
                        options.UseClustering();
                        options.UseJsonSerializer();
                    });
                });
                services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
            }

            services.AddRequiredServices();
            services.Configure<SystemConfig>(Configuration.GetSection(SystemConfig.SectionName));
            services.AddCultureInfo(Configuration);
            Configuration.SetDefaultPageSize();

            var appContext = services.AddMrCMSApps(Configuration, context =>
            {
                context.RegisterApp<MrCMSAdmin>();
                context.RegisterApp<MrCMSCoreApp>();
                context.RegisterApp<MrCMSArticlesApp>();
                context.RegisterTheme<RedTheme>();
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

            services.RegisterSiteLocator();
            services.RegisterSettings();
            services.RegisterShortCodeRenderers();
            services.RegisterFormRenderers();
            services.RegisterTokenProviders();
            services.RegisterDocumentMetadata();
            services.RegisterRouteTransformers();
            services.AddSingleton<IDocumentMetadataService, DocumentMetadataService>();
            services.RegisterTasks();

            services.AddMvcForMrCMS(appContext);
            services.AddSingleton<ICmsMethodTester, CmsMethodTester>();
            services.AddSingleton<IGetMrCMSParts, GetMrCMSParts>();
            services.AddSingleton<IAssignPageDataToRouteValues, AssignPageDataToRouteValues>();
            services.AddSingleton<IQuerySerializer, QuerySerializer>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });

            services.AddScoped(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", builder => builder.RequireRole(UserRole.Administrator));
                appContext.ConfigureAuthorization(options);
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                //options.ExcludedHosts.Add("example.com");
                //options.ExcludedHosts.Add("www.example.com");
            });


            // startup services
            services.AddHostedService<StartupService>();
            services.AddHostedService<RecoverErroredJobsService>();
        }

        private bool IsInstalled()
        {
            var dbSection = Configuration.GetConnectionString("mrcms");
            return dbSection?.Length > 0;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory,
            // NHibernate.ISessionFactory factory,
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            // if (Configuration["EnableHibernatingRhinos"]?.ToLower() == "true")
            //     HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();


            app.UseStatusCodePagesWithReExecute("/HandleStatusCode/{0}");

            app.UseSession();

            if (!IsInstalled())
            {
                app.ShowInstallation();
                return;
            }

            loggerFactory.AddProvider(
                new MrCMSDatabaseLoggerProvider(serviceProvider.GetRequiredService<ISessionFactory>(),
                    httpContextAccessor));

            app.UseMrCMS(builder =>
            {
                builder.UseRequestLocalization();
                builder.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = (context) =>
                    {
                        var headers = context.Context.Response.GetTypedHeaders();
                        headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = env.IsDevelopment() ? TimeSpan.FromDays(0) : TimeSpan.FromDays(30)
                        };
                    }
                });
                builder.UseAuthentication();
            }, builder => { builder.MapRazorPages(); });


            // we're doing DB up here because it happens before startup of hosted services
            // - otherwise Quartz init doesn't happen, among other potential problems
            QuartzConfig.Initialize(Configuration.GetConnectionString("mrcms"));
        }
    }
}