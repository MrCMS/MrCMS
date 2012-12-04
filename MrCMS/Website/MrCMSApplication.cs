using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.IoC;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using MrCMS.Website.Routing;
using NHibernate;
using Ninject;
using Ninject.Web.Common;
using System.Linq;

[assembly: WebActivator.PreApplicationStartMethod(typeof(MrCMSApplication), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(MrCMSApplication), "Stop")]

namespace MrCMS.Website
{
    public abstract class MrCMSApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            RegisterServices(bootstrapper.Kernel);

            ModelBinders.Binders.DefaultBinder = new MrCMSDefaultModelBinder(Get<ISession>);
        }

        public static ISession OverriddenSession { get; set; }
        public static User OverriddenUser { get; set; }

        public override void Init()
        {
            if (DatabaseIsInstalled)
                TaskExecutor.SessionFactory = Get<ISessionFactory>();

            EndRequest += (sender, args) => TaskExecutor.StartExecuting();
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public abstract string RootNamespace { get; }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("InstallerRoute", "install", new { controller = "Install", action = "Setup" });
            routes.MapRoute("Sitemap", "sitemap.xml", new { controller = "SEO", action = "Sitemap" });
            routes.MapRoute("robots.txt", "robots.txt", new { controller = "SEO", action = "Robots" });
            routes.MapRoute("404 route", "404", new { controller = "Error", action = "404" });
            routes.MapRoute("500 route", "500", new { controller = "Error", action = "500" });

            routes.MapRoute("Reset Complete", "reset-complete", new { controller = "Login", action = "ResetComplete" },
                            new[] { RootNamespace });
            routes.MapRoute("Reset Password", "reset-password", new { controller = "Login", action = "PasswordReset" },
                            new[] { RootNamespace });
            routes.MapRoute("Forgotten Password Sent", "forgotten-password-sent", new { controller = "Login", action = "ForgottenPasswordSent" },
                            new[] { RootNamespace });
            routes.MapRoute("Forgotten Password", "forgotten-password", new { controller = "Login", action = "ForgottenPassword" },
                            new[] { RootNamespace });
            routes.MapRoute("Login", "login", new { controller = "Login", action = "Login" },
                            new[] { RootNamespace });
            routes.MapRoute("Logout", "logout", new { controller = "Login", action = "Logout" },
                            new[] { RootNamespace });

            routes.MapRoute("zones", "render-widget", new { action = "Show", controller = "Widget" },
                            new[] { RootNamespace });

            routes.MapRoute("ajax content save", "admintools/savebodycontent",
                            new { controller = "AdminTools", action = "SaveBodyContent" });

            routes.MapRoute("form save", "save-form/{id}", new { controller = "Form", action = "Save" });

            RegisterAppSpecificRoutes(routes);

            routes.Add(new Route("{*data}", new RouteValueDictionary
                                                {

                                                }, new RouteValueDictionary { },
                                 GetConstraints(),
                                 new MrCMSRouteHandler(Get<IDocumentService>,
                                                       Get<SiteSettings>)));
        }

        protected virtual RouteValueDictionary GetConstraints()
        {
            return new RouteValueDictionary
                       {
                           {"Namespaces", new string[] {"MrCMS.Web.Controllers"}}
                       };
        }

        public static bool InDevelopment
        {
            get
            {
                return "true".Equals(ConfigurationManager.AppSettings["Development"],
                                     StringComparison.OrdinalIgnoreCase);
            }
        }

        protected abstract void RegisterAppSpecificRoutes(RouteCollection routes);

        public static Layout GetDefaultLayout(Webpage page)
        {
            return Get<IDocumentService>().GetDefaultLayout(page);
        }

        public static User CurrentUser
        {
            get { return OverriddenUser ?? Get<IUserService>().GetCurrentUser(CurrentContext); }
        }

        public static bool UserLoggedIn
        {
            get { return CurrentUser != null; }
        }

        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new ServiceModule(),
                                            new NHibernateModule(DatabaseType.Auto, InDevelopment));
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        protected abstract void RegisterServices(IKernel kernel);

        public static IEnumerable<T> GetAll<T>()
        {
            return bootstrapper.Kernel.GetAll<T>();
        }

        public static T Get<T>()
        {
            return bootstrapper.Kernel.Get<T>();
        }

        public static object Get(Type type)
        {
            return bootstrapper.Kernel.Get(type);
        }

        public static IEnumerable<Webpage> PublishedRootChildren
        {
            get { return RootChildren.Where(webpage => webpage.Published); }
        }

        public static IEnumerable<Webpage> RootChildren
        {
            get
            {
                return
                    (OverriddenSession ?? Get<ISession>()).QueryOver<Webpage>().Where(document => document.Parent == null).OrderBy(x => x.DisplayOrder).Asc.Cacheable().
                        List();
            }
        }

        public static Webpage CurrentPage
        {
            get { return (Webpage)CurrentContext.Items["current.webpage"]; }
            set { CurrentContext.Items["current.webpage"] = value; }
        }

        public static HttpContextBase CurrentContext
        {
            get { return OverridenContext ?? new HttpContextWrapper(HttpContext.Current); }
        }

        public static HttpContextBase OverridenContext { get; set; }

        public static bool CurrentUserIsAdmin
        {
            get { return CurrentContext.User.IsInRole("Administrator"); }
        }

        public static SiteSettings SiteSettings
        {
            get { return Get<SiteSettings>(); }
        }

        private static bool? _databaseIsInstalled;

        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                {
                    var applicationPhysicalPath = HostingEnvironment.ApplicationPhysicalPath;

                    var connectionStrings = Path.Combine(applicationPhysicalPath, "ConnectionStrings.config");

                    if (!File.Exists(connectionStrings))
                    {
                        File.WriteAllText(connectionStrings, "<connectionStrings></connectionStrings>");
                    }

                    var connectionString = ConfigurationManager.ConnectionStrings["mrcms"];
                    _databaseIsInstalled = connectionString != null &&
                                           !String.IsNullOrEmpty(connectionString.ConnectionString);
                }
                return _databaseIsInstalled.Value;
            }
            set { _databaseIsInstalled = value; }
        }
    }
}