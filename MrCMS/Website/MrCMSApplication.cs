using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.IoC;
using MrCMS.IoC.Modules;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tasks;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Caching;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using MrCMS.Website.Routing;
using NHibernate;
using Ninject;
using Ninject.Web.Common;
using WebActivatorEx;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MrCMSApplication), "Start", Order = 1)]
[assembly: ApplicationShutdownMethod(typeof(MrCMSApplication), "Stop")]

namespace MrCMS.Website
{
    public abstract class MrCMSApplication : HttpApplication
    {
        public const string AssemblyVersion = "0.4.3.0";
        public const string AssemblyFileVersion = "0.4.3.0";
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
        private static IKernel _kernel;
        private const string CachedMissingItemKey = "cached-missing-item";

        protected static IEnumerable<string> WebExtensions
        {
            get
            {
                return Get<SiteSettings>().WebExtensionsToRoute;
            }
        }

        public abstract string RootNamespace { get; }

        public static bool InDevelopment
        {
            get
            {
                return "true".Equals(ConfigurationManager.AppSettings["Development"],
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        private static IKernel Kernel
        {
            get { return _kernel ?? bootstrapper.Kernel; }
        }

        protected void Application_Start()
        {
            MrCMSApp.RegisterAllApps();
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            RegisterServices(bootstrapper.Kernel);
            MrCMSApp.RegisterAllServices(bootstrapper.Kernel);


            SetModelBinders();

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Insert(0, new MrCMSRazorViewEngine());

            ControllerBuilder.Current.SetControllerFactory(new MrCMSControllerFactory());

            GlobalFilters.Filters.Add(new HoneypotFilterAttribute());

            ModelMetadataProviders.Current = new MrCMSMetadataProvider(Kernel);
        }

        private static void SetModelBinders()
        {
            ModelBinders.Binders.DefaultBinder = new MrCMSDefaultModelBinder(Kernel);
            ModelBinders.Binders.Add(typeof(DateTime), new CultureAwareDateBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableCultureAwareDateBinder());
        }

        private static bool IsFileRequest(Uri uri)
        {
            string absolutePath = uri.AbsolutePath;
            if (string.IsNullOrWhiteSpace(absolutePath))
                return false;
            string extension = Path.GetExtension(absolutePath);

            return !string.IsNullOrWhiteSpace(extension) && !WebExtensions.Contains(extension);
        }

        public override void Init()
        {
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                BeginRequest += (sender, args) =>
                {
                    if (IsCachedMissingFileRequest()) return;
                    CurrentRequestData.ErrorSignal = ErrorSignal.FromCurrentContext();
                    if (!IsFileRequest(Request.Url))
                    {
                        CurrentRequestData.CurrentSite = Get<ICurrentSiteLocator>().GetCurrentSite();
                        CurrentRequestData.SiteSettings = Get<SiteSettings>();
                        CurrentRequestData.HomePage = Get<IGetHomePage>().Get();
                        Thread.CurrentThread.CurrentCulture = CurrentRequestData.SiteSettings.CultureInfo;
                        Thread.CurrentThread.CurrentUICulture = CurrentRequestData.SiteSettings.CultureInfo;
                    }
                };
                AuthenticateRequest += (sender, args) =>
                {
                    if (!Context.Items.Contains(CachedMissingItemKey) && !IsFileRequest(Request.Url))
                    {
                        if (CurrentRequestData.CurrentContext.User != null)
                        {
                            User currentUser = Get<IUserService>().GetCurrentUser(CurrentRequestData.CurrentContext);
                            if (!Request.Url.AbsolutePath.StartsWith("/signalr/") && (currentUser == null ||
                                !currentUser.IsActive))
                                Get<IAuthorisationService>().Logout();
                            else
                            {
                                CurrentRequestData.CurrentUser = currentUser;
                                Thread.CurrentThread.CurrentCulture = currentUser.GetUICulture();
                                Thread.CurrentThread.CurrentUICulture = currentUser.GetUICulture();
                            }
                        }
                    }
                };
                EndRequest += (sender, args) =>
                {
                    if (Context.Items.Contains(CachedMissingItemKey))
                        return;
                    if (CurrentRequestData.QueuedTasks.Any())
                    {
                        Kernel.Get<ISession>()
                            .Transact(session =>
                            {
                                foreach (QueuedTask queuedTask in CurrentRequestData.QueuedTasks)
                                    session.Save(queuedTask);
                            });
                    }
                    foreach (var action in CurrentRequestData.OnEndRequest)
                        action(Kernel);
                };
            }
            else
            {
                EndRequest += (sender, args) =>
                {
                    foreach (var action in CurrentRequestData.OnEndRequest)
                        action(Kernel);
                };
            }
        }

        private bool IsCachedMissingFileRequest()
        {
            var o = Get<ICacheWrapper>()[FileNotFoundHandler.GetMissingFileCacheKey(new HttpContextWrapper(Context))];
            if (o != null)
            {
                Context.Items[CachedMissingItemKey] = true;
                Context.ApplicationInstance.CompleteRequest();
                return true;
            }
            return false;
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("InstallerRoute", "install", new { controller = "Install", action = "Setup" });
            routes.MapRoute("Task Execution", "execute-pending-tasks",
                new { controller = "TaskExecution", action = "Execute" });
            routes.MapRoute("Sitemap", "sitemap.xml", new { controller = "SEO", action = "Sitemap" });
            routes.MapRoute("robots.txt", "robots.txt", new { controller = "SEO", action = "Robots" });
            routes.MapRoute("ckeditor Config", "Areas/Admin/Content/Editors/ckeditor/config.js",
                new { controller = "CKEditor", action = "Config" });

            routes.MapRoute("Logout", "logout", new { controller = "Login", action = "Logout" },
                new[] { RootNamespace });

            routes.MapRoute("zones", "render-widget", new { controller = "Widget", action = "Show" },
                new[] { RootNamespace });

            routes.MapRoute("ajax content save", "admintools/savebodycontent",
                new { controller = "AdminTools", action = "SaveBodyContent" });

            routes.MapRoute("form save", "save-form/{id}", new { controller = "Form", action = "Save" },
                new[] { typeof(FormController).Namespace });

            routes.Add(new Route("{*data}", new RouteValueDictionary(), new RouteValueDictionary(),
                new MrCMSRouteHandler()));
        }

        /// <summary>
        ///     Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        ///     Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        ///     Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel(new NinjectSettings { AllowNullInjection = true },
                new ServiceModule(),
                new SettingsModule(),
                new FileSystemModule(),
                new GenericBindingsModule(),
                new SystemWebModule(),
                new SiteModule(),
                new AuthModule());
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            kernel.Load(new NHibernateModule());
            return kernel;
        }

        /// <summary>
        ///     Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        protected abstract void RegisterServices(IKernel kernel);

        public static IEnumerable<T> GetAll<T>()
        {
            return Kernel.GetAll<T>();
        }

        public static void OverrideKernel(IKernel kernel)
        {
            _kernel = kernel;
        }

        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        public static object Get(Type type)
        {
            return Kernel.Get(type);
        }
    }
}