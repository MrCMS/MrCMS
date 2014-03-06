using System;
using System.Collections.Generic;
using System.Web.Routing;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Installation;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using Ninject;

namespace MrCMS.Apps
{
    public abstract class MrCMSApp
    {
        protected abstract void RegisterApp(MrCMSAppRegistrationContext context);

        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public abstract string AppName { get; }
        public abstract string Version { get; }

        public static string CurrentAppSummary
        {
            get { return string.Join(", ", AllApps.Select(app => app.AppName + ": " + app.Version)); }
        }

        public static readonly Dictionary<Type, string> AppWebpages = new Dictionary<Type, string>();
        public static readonly Dictionary<Type, string> AppWidgets = new Dictionary<Type, string>();
        public static readonly Dictionary<Type, string> AppUserProfileDatas = new Dictionary<Type, string>();
        public static readonly Dictionary<Type, string> AppEntities = new Dictionary<Type, string>();
        public static readonly Dictionary<Type, string> AllAppTypes = new Dictionary<Type, string>();

        static MrCMSApp()
        {
            AllApps.ForEach(app =>
            {
                var webpageTypes =
                    TypeHelper.GetAllConcreteTypesAssignableFrom<Webpage>()
                              .FindAll(type => type.Namespace.StartsWith(app.GetType().Namespace));
                webpageTypes.ForEach(type => AppWebpages[type] = app.AppName);
                var widgetTypes =
                    TypeHelper.GetAllConcreteTypesAssignableFrom<Widget>()
                              .FindAll(type => type.Namespace.StartsWith(app.GetType().Namespace));
                widgetTypes.ForEach(type => AppWidgets[type] = app.AppName);
                var userProfileTypes =
                    TypeHelper.GetAllConcreteTypesAssignableFrom<UserProfileData>()
                              .FindAll(type => type.Namespace.StartsWith(app.GetType().Namespace));
                userProfileTypes.ForEach(type => AppUserProfileDatas[type] = app.AppName);
                var entities =
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>()
                              .FindAll(type => type.Namespace.StartsWith(app.GetType().Namespace));
                entities.ForEach(type => AppEntities[type] = app.AppName);
                var types =
                    TypeHelper.GetAllTypes()
                              .Where(
                                  type =>
                                  !string.IsNullOrWhiteSpace(type.Namespace) &&
                                  type.Namespace.StartsWith(app.GetType().Namespace));
                types.ForEach(type => AllAppTypes[type] = app.AppName);
            });
        }
        public static Dictionary<Type, string> AppTypes
        {
            get { return AllAppTypes.Where(pair => !pair.Key.IsAbstract).ToDictionary(pair => pair.Key, pair => pair.Value); }
        }
        public static Dictionary<Type, string> AbstractTypes
        {
            get { return AllAppTypes.Where(pair => pair.Key.IsAbstract).ToDictionary(pair => pair.Key, pair => pair.Value); }
        }
        private static List<MrCMSApp> _allApps;
        public virtual IEnumerable<Type> BaseTypes { get { yield break; } }
        public virtual IEnumerable<Type> Conventions { get { yield break; } }

        internal void CreateContext(RouteCollection routes, object state)
        {
            var context = new MrCMSAppRegistrationContext(this.AppName, routes, state);
            string @namespace = this.GetType().Namespace;
            if (@namespace != null)
                context.Namespaces.Add(@namespace + ".*");
            this.RegisterApp(context);
        }

        /// <summary>
        /// Registers all areas in an ASP.NET MVC application by using the specified user-defined information.
        /// </summary>
        /// <param name="state">An object that contains user-defined information to pass to the area.</param>
        public static void RegisterAllApps(object state = null)
        {
            RegisterAllApps(RouteTable.Routes, state);
        }

        private static void RegisterAllApps(RouteCollection routes, object state)
        {
            AllApps.ForEach(app => app.CreateContext(routes, state));
        }

        public static void RegisterAllServices(IKernel kernel)
        {
            AllApps.ForEach(app => app.RegisterServices(kernel));
        }

        public static void InstallApps(ISession session, InstallModel model, Site site)
        {
            AllApps.OrderBy(app => app.InstallOrder).ForEach(app => app.OnInstallation(session, model, site));
        }

        private static List<MrCMSApp> AllApps
        {
            get
            {
                return _allApps = _allApps ?? TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSApp>()
                                                        .Select(type => ((MrCMSApp)Activator.CreateInstance(type))).ToList();
            }
        }

        public static void AppendAllAppConfiguration(Configuration configuration)
        {
            AllApps.ForEach(app => app.AppendConfiguration(configuration));
        }

        protected virtual void AppendConfiguration(Configuration configuration) { }

        protected virtual int InstallOrder { get { return 10; } }

        public static IEnumerable<string> AppNames { get { return AllApps.Select(app => app.AppName); } }


        protected abstract void RegisterServices(IKernel kernel);

        protected abstract void OnInstallation(ISession session, InstallModel model, Site site);
    }
}