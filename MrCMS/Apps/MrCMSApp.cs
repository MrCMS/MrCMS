using System;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Helpers;

namespace MrCMS.Apps
{
    public abstract class MrCMSApp
    {
        protected abstract void RegisterApp(MrCMSAppRegistrationContext context);

        private const string _typeCacheName = "MVC-AreaRegistrationTypeCache.xml";

        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// 
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        protected abstract string AppName { get; }

        internal void CreateContextAndRegister(RouteCollection routes, object state)
        {
            var context = new MrCMSAppRegistrationContext(this.AppName, routes, state);
            string @namespace = this.GetType().Namespace;
            if (@namespace != null)
                context.Namespaces.Add(@namespace + ".*");
            this.RegisterApp(context);
        }

        private static bool IsAppRegistrationType(Type type)
        {
            return typeof(MrCMSApp).IsAssignableFrom(type) &&
                   type.GetConstructor(Type.EmptyTypes) != null;
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
            foreach (Type type in TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSApp>())
                ((MrCMSApp)Activator.CreateInstance(type)).CreateContextAndRegister(routes, state);
        }
    }
}