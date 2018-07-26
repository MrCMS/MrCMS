using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MrCMS.Apps;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin;
using MrCMS.Web.Apps.Core;
using MrCMS.Website;
using MrCMS.Website.CMS;
using NHibernate;
using ISession = NHibernate.ISession;

namespace MrCMS.Web
{
    public class Startup
    {
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


            var appContext = services.AddMrCMSApps(context =>
            {
                context.RegisterApp<MrCMSCoreApp>();
                context.RegisterApp<MrCMSAdminApp>();
            });
            services.AddSingleton(appContext);

            var physicalProvider = Environment.ContentRootFileProvider;
            var compositeFileProvider =
                new CompositeFileProvider(new[] { physicalProvider }.Concat(appContext.ViewFileProviders));

            services.AddSingleton<IFileProvider>(compositeFileProvider);

            services.AddMvc(options =>
                {
                    // add custom binder to beginning of collection
                    options.ModelBinderProviders.Insert(0, new SystemEntityBinderProvider());
                }).AddRazorOptions(options =>
                {
                    options.ViewLocationExpanders.Insert(0, new WebpageViewExpander());
                    options.ViewLocationExpanders.Insert(1, new AppViewLocationExpander());
                    options.FileProviders.Add(compositeFileProvider);
                })
                .AddAppMvcConfig(appContext);


            services.AddSingleton<ISessionFactory>(provider => new NHibernateConfigurator(new SqlServer2012Provider(
                new DatabaseSettings
                {
                    ConnectionString =
                        "Data Source=localhost;Initial Catalog=mrcms-migration;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True"
                })).CreateSessionFactory());
            services.AddScoped<ISession>(provider => provider.GetRequiredService<ISessionFactory>().OpenSession());

            services.AddSingleton<ICmsMethodTester, CmsMethodTester>();
            services.AddSingleton<IAssignPageDataToRouteData, AssignPageDataToRouteData>();
            services.AddSingleton<IQuerySerializer, QuerySerializer>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // TODO: update the resolution of this
            services.AddSingleton<IFileSystem, FileSystem>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, MrCMSAppContext appContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new CompositeFileProvider(
                    new[] { Environment.WebRootFileProvider }.Concat(appContext.ContentFileProviders))
            });

            app.UseMvc(builder =>
            {
                builder.MapMrCMS();
                builder.MapMrCMSApps(appContext);

                builder.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }

    public class AppViewLocationExpander : IViewLocationExpander
    {
        // TODO: implement properly
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["app"] = "Core";
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Select(s =>
            {
                if (s.IndexOf("~") == 0)
                    return s.Replace("~/", $"~/Apps/{context.Values["app"]}/");
                return $"/Apps/{context.Values["app"]}" + s;
            }).Concat(viewLocations);
        }
    }

    public class WebpageViewExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (!context.IsMainPage) return;
            if (!(context.ActionContext.HttpContext.Items[ProcessWebpageViews.CurrentPage] is Webpage webpage)) return;
            context.Values["webpage"] = webpage.DocumentType;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (!context.Values.ContainsKey("webpage"))
                return viewLocations;

            return viewLocations.Prepend($"~/Views/Pages/{context.Values["webpage"]}.cshtml");
        }
    }
}
