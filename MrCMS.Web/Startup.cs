using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin;
using MrCMS.Web.Apps.Core;
using MrCMS.Web.Apps.Core.Auth;
using MrCMS.Website;
using MrCMS.Website.CMS;
using NHibernate;
using System;
using System.Linq;
using ISession = NHibernate.ISession;

namespace MrCMS.Web
{
    public class Startup
    {
        private const string Database = nameof(Database);
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;

            TypeHelper.Initialize(GetType().Assembly);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterAllSimplePairings();
            services.RegisterOpenGenerics();
            services.SelfRegisterAllConcreteTypes();
            services.AddSession();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var appContext = services.AddMrCMSApps(context =>
            {
                context.RegisterApp<MrCMSCoreApp>();
                context.RegisterApp<MrCMSAdminApp>();
            });
            services.AddSingleton(appContext);

            services.AddScoped<ISession>(provider =>
            {
                var session = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-nh-session"] as ISession;
                return session ?? provider.GetRequiredService<ISessionFactory>().OpenFilteredSession(provider);
            });
            services.AddTransient<IStatelessSession>(provider =>
            {
                var session = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-nh-stateless-session"] as IStatelessSession;
                return session ?? provider.GetRequiredService<ISessionFactory>().OpenStatelessSession();
            });
            // TODO: Look to removing Site for constructors and resolving like this
            services.AddScoped<Site>(provider =>
            {
                var site = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Items["override-site"] as Site;
                return site ?? provider.GetRequiredService<ICurrentSiteLocator>().GetCurrentSite();
            });

            services.AddScoped<IFileSystem>(provider =>
            {
                var settings = provider.GetService<FileSystemSettings>();

                var storageType = settings.StorageType;
                if (string.IsNullOrWhiteSpace(storageType))
                {
                    return provider.GetService<FileSystem>();
                }

                var type = TypeHelper.GetTypeByName(storageType);
                if (type?.IsAssignableFrom(typeof(IFileSystem)) != true)
                {
                    return provider.GetService<FileSystem>();
                }

                return provider.GetService(type) as IFileSystem;
            });

            services.AddSignalR();

            if (!IsInstalled())
            {
                services.AddSingleton((IFileProvider)new InstallationContentFileProvider());

                services.AddMvc().AddRazorOptions(options =>
                {
                var fileProvider = new InstallationViewFileProvider();
                    options.FileProviders.Insert(0, fileProvider);
                });
                return;
            }
            var physicalProvider = Environment.ContentRootFileProvider;
            var compositeFileProvider =
                new CompositeFileProvider(new[] { physicalProvider }.Concat(appContext.ViewFileProviders));

            services.AddSingleton<IFileProvider>(compositeFileProvider);

            services.RegisterSettings();
            services.RegisterTokenProviders();
            services.RegisterTasks();


            services.AddAutoMapper(expression =>
            {
                expression.AllowNullDestinationValues = true;
                appContext.ConfigureAutomapper(expression);
            });

            services.AddMvc(options =>
                {
                    // add custom binder to beginning of collection
                    options.ModelBinderProviders.Insert(0, new SystemEntityBinderProvider());
                    appContext.SetupMvcOptions(options);

                }).AddRazorOptions(options =>
                {
                    options.ViewLocationExpanders.Insert(0, new WebpageViewExpander());
                    options.ViewLocationExpanders.Insert(1, new AppViewLocationExpander());
                    options.FileProviders.Add(compositeFileProvider);
                })
                .AddViewLocalization()
                .AddDataAnnotations()
                .AddDataAnnotationsLocalization()
                .AddAppMvcConfig(appContext);

            services.AddLogging(builder => builder.AddMrCMSLogger());

            services.Configure<DatabaseSettings>(Configuration.GetSection(Database));
            services.RegisterDatabaseProviders();

            services.AddSingleton<ISessionFactory>(provider =>
                new NHibernateConfigurator(provider.GetRequiredService<IDatabaseProviderResolver>().GetProvider(),
                    provider.GetRequiredService<MrCMSAppContext>()).CreateSessionFactory());

            services.AddIdentity<User, UserRole>(options =>
            {
                // lockout
                if (int.TryParse(Configuration["Auth:Lockout:MaxFailedAccessAttempts"],
                    out var maxFailedAccessAttempts))
                {
                    options.Lockout.MaxFailedAccessAttempts = maxFailedAccessAttempts;
                }

                if (TimeSpan.TryParse(Configuration["Auth:Lockout:DefaultLockoutTimeSpan"],
                    out var defaultLockoutTimeSpan))
                {
                    options.Lockout.DefaultLockoutTimeSpan = defaultLockoutTimeSpan;
                }

                if (bool.TryParse(Configuration["Auth:Lockout:AllowedForNewUsers"], out var allowedForNewUsers))
                {
                    options.Lockout.AllowedForNewUsers = allowedForNewUsers;
                }

                // password
                if (int.TryParse(Configuration["Auth:Password:RequiredLength"], out var requiredLength))
                {
                    options.Password.RequiredLength = requiredLength;
                }

                if (bool.TryParse(Configuration["Auth:Password:RequireDigit"], out var requireDigit))
                {
                    options.Password.RequireDigit = requireDigit;
                }

                if (bool.TryParse(Configuration["Auth:Password:RequireLowercase"], out var requireLowercase))
                {
                    options.Password.RequireLowercase = requireLowercase;
                }

                if (bool.TryParse(Configuration["Auth:Password:RequireNonAlphanumeric"],
                    out var requireNonAlphanumeric))
                {
                    options.Password.RequireNonAlphanumeric = requireNonAlphanumeric;
                }

                if (bool.TryParse(Configuration["Auth:Password:RequireUppercase"], out var requireUppercase))
                {
                    options.Password.RequireUppercase = requireUppercase;
                }

                // sign in
                if (bool.TryParse(Configuration["Auth:SignIn:RequireConfirmedEmail"],
                    out var requireConfirmedEmail))
                {
                    options.SignIn.RequireConfirmedEmail = requireConfirmedEmail;
                }

                if (bool.TryParse(Configuration["Auth:SignIn:RequireConfirmedPhoneNumber"],
                    out var requireConfirmedPhoneNumber))
                {
                    options.SignIn.RequireConfirmedPhoneNumber = requireConfirmedPhoneNumber;
                }

                options.User.RequireUniqueEmail = true;
                options.ClaimsIdentity.UserNameClaimType = nameof(User.Email);
                options.ClaimsIdentity.UserIdClaimType = nameof(User.Id);
            })
              .AddMrCMSStores()
              .AddDefaultTokenProviders();


            services.AddSingleton<ICmsMethodTester, CmsMethodTester>();
            services.AddSingleton<IGetMrCMSParts, GetMrCMSParts>();
            services.AddSingleton<IAssignPageDataToRouteData, AssignPageDataToRouteData>();
            services.AddSingleton<IQuerySerializer, QuerySerializer>();
            services.AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IDatabaseProviderResolver, DatabaseProviderResolver>();


            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
            services.AddScoped(provider => provider.GetService<IStringLocalizerFactory>()
                .Create(null, null));


            services.AddAuthentication();
            services.TryAddEnumerable(ServiceDescriptor.Transient<IPostConfigureOptions<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache>());
            services.AddSingleton<IOptionsMonitorCache<CookieAuthenticationOptions>, GetCookieAuthenticationOptionsFromCache>();
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MrCMSAppContext appContext, IDatabaseProviderResolver databaseProviderResolver)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();


            if (!databaseProviderResolver.IsProviderConfigured())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new InstallationContentFileProvider()
                });

                app.ShowInstallation();
            }
            else
            {
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

}
