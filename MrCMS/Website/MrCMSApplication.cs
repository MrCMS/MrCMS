using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MrCMS.Apps;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.IoC;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
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
            MrCMSApp.RegisterAllApps();
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            RegisterServices(bootstrapper.Kernel);

            ModelBinders.Binders.DefaultBinder = new MrCMSDefaultModelBinder(Get<ISession>);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Insert(0, new MrCMSRazorViewEngine());

            ControllerBuilder.Current.SetControllerFactory(new MrCMSControllerFactory());
        }

        public static User OverriddenUser { get; set; }

        public override void Init()
        {
            if (DatabaseIsInstalled)
                TaskExecutor.SessionFactory = Get<ISessionFactory>();

            EndRequest += (sender, args) =>
            {
                if (DatabaseIsInstalled)
                    AppendScheduledTasks();
                TaskExecutor.StartExecuting();
            };
        }

        protected void AppendScheduledTasks()
        {
            var scheduledTaskManager = Get<IScheduledTaskManager>();
            foreach (var scheduledTask in scheduledTaskManager.GetDueTasks())
                TaskExecutor.ExecuteLater(scheduledTaskManager.GetTask(scheduledTask));
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static ErrorSignal ErrorSignal
        {
            get { return OverridenSignal ?? Elmah.ErrorSignal.FromCurrentContext(); }
        }

        public static ErrorSignal OverridenSignal { get; set; }

        public abstract string RootNamespace { get; }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("InstallerRoute", "install", new { controller = "Install", action = "Setup" });
            routes.MapRoute("Sitemap", "sitemap.xml", new { controller = "SEO", action = "Sitemap" });
            routes.MapRoute("robots.txt", "robots.txt", new { controller = "SEO", action = "Robots" });

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

            routes.Add(new Route("{*data}", new RouteValueDictionary(), new RouteValueDictionary(), GetConstraints(),
                                 new MrCMSRouteHandler(Get<ISession>, Get<IDocumentService>, Get<SiteSettings>)));
        }

        protected virtual RouteValueDictionary GetConstraints()
        {
            return new RouteValueDictionary
                       {
                           {"Namespaces", new[] {"MrCMS.Web.Controllers"}}
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

        public static Layout OverridenDefaultLayout { get; set; }
        public static Layout GetDefaultLayout(Webpage page)
        {
            return OverridenDefaultLayout ?? Get<IDocumentService>().GetDefaultLayout(page);
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

        public static IEnumerable<Webpage> PublishedRootChildren()
        {
            return RootChildren().Where(webpage => webpage.Published);
        }

        public static IEnumerable<Webpage> OverridenRootChildren { get; set; }
        public static IEnumerable<Webpage> RootChildren()
        {
            return OverridenRootChildren ??
                   Get<ISession>()
                       .QueryOver<Webpage>()
                       .Where(document => document.Parent == null && document.Site == CurrentSite)
                       .OrderBy(x => x.DisplayOrder)
                       .Asc.Cacheable()
                       .List();
        }

        public static Site CurrentSite
        {
            get { return OverriddenSite ?? Get<ISiteService>().GetCurrentSite(); }
        }

        public static Site OverriddenSite { get; set; }

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
            get { return CurrentUser != null && CurrentUser.IsAdmin; }
        }

        public static SiteSettings OverriddenSiteSettings { get; set; }
        public static SiteSettings SiteSettings
        {
            get { return OverriddenSiteSettings ?? Get<SiteSettings>(); }
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


        public const string AssemblyVersion = "0.2.0.*";
        public const string AssemblyFileVersion = "0.2.0.0";
    }

    public class MrCMSControllerFactory : DefaultControllerFactory
    {
        private Dictionary<string, List<Type>> _appUiControllers;
        private Dictionary<string, List<Type>> _appAdminControllers;
        private List<Type> _uiControllers;
        private List<Type> _adminControllers;

        public MrCMSControllerFactory()
        {
            _appUiControllers =
                TypeHelper.GetAllConcreteTypesAssignableFrom(typeof (MrCMSAppUIController<>))
                          .GroupBy(
                              type =>
                              ((MrCMSApp) Activator.CreateInstance(
                                  type.GetBaseTypes(
                                      type1 =>
                                      type1.IsGenericType &&
                                      type1.GetGenericTypeDefinition() == typeof(MrCMSAppUIController<>))
                                      .First()
                                      .GetGenericArguments()[0])).AppName)
                          .ToDictionary(types => types.Key, types => types.ToList());
            _appAdminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof (MrCMSAppAdminController<>))
                                             .GroupBy(
                                                 type =>
                                                 ((MrCMSApp) Activator.CreateInstance(
                                                     type.GetBaseTypes(
                                                         type1 =>
                                                         type1.IsGenericType &&
                                                         type1.GetGenericTypeDefinition() ==
                                                         typeof (MrCMSAppAdminController<>))
                                                         .First()
                                                         .GetGenericArguments()[0])).AppName)
                                             .ToDictionary(types => types.Key, types => types.ToList());
            _uiControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSUIController>();
            _adminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSAdminController>();
        }
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            if (requestContext.RouteData.DataTokens["app"] != null)
            {
                if ("admin".Equals(Convert.ToString(requestContext.RouteData.DataTokens["area"]),
                                   StringComparison.OrdinalIgnoreCase))
                    return _appAdminControllers[requestContext.RouteData.DataTokens["app"].ToString()].SingleOrDefault(
                        type =>
                        type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
                else
                    return _appUiControllers[requestContext.RouteData.DataTokens["app"].ToString()].SingleOrDefault(
                        type =>
                        type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            }

            if ("admin".Equals(Convert.ToString(requestContext.RouteData.DataTokens["area"]),
                               StringComparison.OrdinalIgnoreCase))
                return _adminControllers.SingleOrDefault(
                    type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            else
                return _uiControllers.SingleOrDefault(
                    type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
        }
    }
}