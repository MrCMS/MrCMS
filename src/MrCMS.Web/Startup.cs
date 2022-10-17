using System;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Web.Admin;
using MrCMS.Web.Apps.Core;
using MrCMS.Website;
using MrCMS.Website.CMS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
            // services.AddSyncDataAccess(Configuration);
            if (Environment.IsProduction())
            {
                var connectionString = Configuration["DataProtectionConnectionString"];
                var keyName = Configuration["DataProtectionKeyName"];
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    services
                        .AddDataProtection()
                        .SetApplicationName("MrCMS")
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
					q.UseDefaultThreadPool();
                    q.UseMicrosoftDependencyInjectionJobFactory();

                    q.UsePersistentStore(options =>
                    {
                        options.UseSqlServer(providerOptions =>
                        {
                            providerOptions.ConnectionString = Configuration.GetConnectionString("mrcms");
                        });
                        //options.UseClustering();
                        options.UseJsonSerializer();
                    });
                });
                services.AddQuartzHostedService(options =>
                {
                    options.WaitForJobsToComplete = true;
                    options.AwaitApplicationStarted = true;
                });
            }

            services.AddRequiredServices();
            services.Configure<SystemConfig>(Configuration.GetSection(SystemConfig.SectionName));
            services.AddCultureInfo(Configuration);
            Configuration.SetDefaultPageSize();
            Configuration.SetMaxFileSize();

            var appContext = services.AddMrCMSApps(Configuration, context =>
            {
                context.RegisterApp<MrCMSAdmin>();
                context.RegisterApp<MrCMSCoreApp>();
                context.RegisterApp<MrCMSArticlesApp>();
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
            services.AddSingleton<IWebpageMetadataService, WebpageMetadataService>();
            services.RegisterTasks();

            services.AddMvcForMrCMS(appContext);
            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = SessionHelper.MaxFileSize;
            });
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
            // services.AddHostedService<SeedHostedServices>();

            Configuration.SetMiniProfilerEnableStatus();
            if (IsMiniProfileEnabled())
            {
                // miniprofiler
                // services.AddMiniProfiler(options =>
                // {
                //     options.RouteBasePath = "/profiler";
                //     options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomRight;
                //     options.PopupShowTimeWithChildren = true;
                //     options.ShouldProfile = x =>
                //     {
                //         // rough filter for assets
                //         if (!x.Path.HasValue)
                //             return true;
                //         // don't profile AJAX requests
                //         if (x.IsAjaxRequest())
                //             return false;
                //
                //         return !x.Path.Value!.EndsWith(".js", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".css", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".map", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".eot", StringComparison.OrdinalIgnoreCase) &&
                //                !x.Path.Value.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);
                //     };
                //     options.ResultsAuthorizeAsync = (context) =>
                //     {
                //         // var getCurrentUser = context.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>();
                //         // var user = await getCurrentUser.GetLoggedInUser();
                //         // return user.IsAdmin;
                //         return Task.FromResult(true);
                //     };
                // }).AddEntityFramework();

                services.AddAntiforgery(options =>
                {
                    // Set Cookie properties using CookieBuilder properties�.
                    options.SuppressXFrameOptionsHeader = false;
                });
            }
        }
        
        private bool IsMiniProfileEnabled()
        {
            var enableMiniProfiler = Configuration.GetValue<bool>("EnableMiniProfiler");
            return enableMiniProfiler;
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
            IServiceProvider serviceProvider, IAntiforgery antiforgery)
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

            if (IsMiniProfileEnabled())
            {
                app.UseMiniProfiler();
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
                
                builder.Use(next => ctx =>
                {
                    var tokens = antiforgery.GetAndStoreTokens(ctx);

                    ctx.Response.Cookies.Append("RequestVerificationToken", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false });

                    return next(ctx);
                });

                builder.Use(async (context, next) =>
                {
                    var maxAllowedFileSize = SessionHelper.MaxFileSize;
                    var contentLength = context.Request.ContentLength ?? 0;
                    if (contentLength > maxAllowedFileSize)
                    {
                        context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                        await context.Response.WriteAsync(
                            $"File is too big ({(contentLength / 1024 / 1024)}MiB). Max filesize: {(maxAllowedFileSize / 1024 / 1024)}MiB.");
                        return;
                    }

                    await next.Invoke();
                });
            }, builder => { builder.MapRazorPages(); });


            QuartzConfig.Initialize(Configuration.GetConnectionString("mrcms"));
        }
    }
}