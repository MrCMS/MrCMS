using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Website
{
    public class MrCMSControllerFactory : DefaultControllerFactory
    {
        public static readonly Dictionary<string, HashSet<Type>> AppUiControllers;
        public static readonly Dictionary<string, HashSet<Type>> AppAdminControllers;
        public static readonly HashSet<Type> UiControllers;
        public static readonly HashSet<Type> AdminControllers;

        static MrCMSControllerFactory()
        {
            AppUiControllers =
                MrCMSApp.AppTypes.Where(pair => typeof(MrCMSUIController).IsAssignableFrom(pair.Key))
                    .GroupBy(pair => pair.Value)
                    .ToDictionary(grouping => grouping.Key,
                        grouping => grouping.Select(pair => pair.Key).ToHashSet());
            AppAdminControllers =
                MrCMSApp.AppTypes.Where(pair => typeof(MrCMSAdminController).IsAssignableFrom(pair.Key))
                    .GroupBy(pair => pair.Value)
                    .ToDictionary(grouping => grouping.Key,
                        grouping => grouping.Select(pair => pair.Key).ToHashSet());
            UiControllers = new HashSet<Type>(
                TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSUIController>()
                    .Where(type => !AppUiControllers.SelectMany(pair => pair.Value).Contains(type)));
            AdminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSAdminController>()
                .Where(type => !AppAdminControllers.SelectMany(pair => pair.Value).Contains(type)).ToHashSet();
        }

        public static IEnumerable<Type> AllControllers
        {
            get
            {
                foreach (Type controller in UiControllers)
                    yield return controller;
                foreach (Type controller in AdminControllers)
                    yield return controller;
                foreach (Type controller in AppUiControllers.Keys.SelectMany(key => AppUiControllers[key]))
                    yield return controller;
                foreach (Type controller in AppAdminControllers.Keys.SelectMany(key => AppAdminControllers[key]))
                    yield return controller;
            }
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            string appName = Convert.ToString(requestContext.RouteData.DataTokens["app"]);
            string areaName = Convert.ToString(requestContext.RouteData.DataTokens["area"]);

            HashSet<Type> listToCheck = "admin".Equals(areaName, StringComparison.OrdinalIgnoreCase)
                ? GetAdminControllersToCheck(appName)
                : GetUiControllersToCheck(appName);

            Type controllerType =
                listToCheck.FirstOrDefault(
                    type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            return controllerType;
        }

        private HashSet<Type> GetAdminControllersToCheck(string appName)
        {
            var types = new HashSet<Type>();
            if (!String.IsNullOrWhiteSpace(appName) && AppAdminControllers.ContainsKey(appName))
                types.AddRange(AppAdminControllers[appName]);
            types.AddRange(AdminControllers);
            foreach (
                string key in
                    AppAdminControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase))
                )
            {
                types.AddRange(AppAdminControllers[key]);
            }
            return types;
        }

        private HashSet<Type> GetUiControllersToCheck(string appName)
        {
            var types = new HashSet<Type>();
            if (!String.IsNullOrWhiteSpace(appName) && AppUiControllers.ContainsKey(appName))
                types.AddRange(AppUiControllers[appName]);
            types.AddRange(UiControllers);
            foreach (
                string key in
                    AppUiControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase)))
            {
                types.AddRange(AppUiControllers[key]);
            }
            return types;
        }

        public bool IsValidControllerType(string appName, string controllerName, bool? isAdmin)
        {
            string typeName = controllerName + "Controller";
            if (!String.IsNullOrWhiteSpace(appName))
            {
                HashSet<Type> adminControllers = GetAdminControllersToCheck(appName);
                HashSet<Type> uiControllers = GetUiControllersToCheck(appName);
                IEnumerable<Type> appControllersToCheck = !isAdmin.HasValue
                    ? adminControllers.AddRange(uiControllers)
                    : isAdmin.Value ? adminControllers : uiControllers;
                return appControllersToCheck.Any(
                    type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            }
            IEnumerable<Type> controllersToCheck = !isAdmin.HasValue
                ? AdminControllers.Concat(UiControllers)
                : isAdmin.Value ? AdminControllers : UiControllers;
            return controllersToCheck.Any(type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<MethodInfo> GetActionMethods(Type controllerType)
        {
            return controllerType.GetMethods()
                .Where(
                    q =>
                        q.IsPublic &&
                        (typeof(ActionResult).IsAssignableFrom(q.ReturnType) ||
                         (q.ReturnType.IsGenericType && q.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) &&
                          q.ReturnType.GetGenericArguments().Count() == 1 &&
                          typeof(ActionResult).IsAssignableFrom(q.ReturnType.GetGenericArguments()[0]))));
        }

        public static List<ActionMethodInfo<T>> GetActionMethodsWithAttribute<T>() where T : Attribute
        {
            IEnumerable<ReflectedControllerDescriptor> reflectedControllerDescriptors =
                AllControllers.Select(type => new ReflectedControllerDescriptor(type));
            IEnumerable<ActionDescriptor> descriptors = reflectedControllerDescriptors.SelectMany(
                controllerDescriptor =>
                    controllerDescriptor.GetCanonicalActions()
                        .Where(actionDescriptor => actionDescriptor.GetCustomAttributes(typeof(T), true).Any()));
            return descriptors.Select(descriptor => new ActionMethodInfo<T>
            {
                Descriptor = descriptor,
                Attribute =
                    descriptor.GetCustomAttributes(typeof(T), true)
                        .FirstOrDefault() as T
            }).ToList();
        }
    }
}