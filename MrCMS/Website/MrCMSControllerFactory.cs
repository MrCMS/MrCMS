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
using Ninject.Infrastructure.Language;

namespace MrCMS.Website
{
    public class MrCMSControllerFactory : DefaultControllerFactory
    {
        public static readonly Dictionary<string, List<Type>> AppUiControllers;
        public static readonly Dictionary<string, List<Type>> AppAdminControllers;
        public static readonly List<Type> UiControllers;
        public static readonly List<Type> AdminControllers;

        public static IEnumerable<Type> AllControllers
        {
            get
            {
                foreach (var controller in UiControllers)
                    yield return controller;
                foreach (var controller in AdminControllers)
                    yield return controller;
                foreach (var controller in AppUiControllers.Keys.SelectMany(key => AppUiControllers[key]))
                    yield return controller;
                foreach (var controller in AppAdminControllers.Keys.SelectMany(key => AppAdminControllers[key]))
                    yield return controller;
            }
        }

        static MrCMSControllerFactory()
        {
            AppUiControllers =
                MrCMSApp.AppTypes.Where(pair => typeof(MrCMSUIController).IsAssignableFrom(pair.Key))
                    .GroupBy(pair => pair.Value)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(pair => pair.Key).ToList());
            AppAdminControllers =
                MrCMSApp.AppTypes.Where(pair => typeof(MrCMSAdminController).IsAssignableFrom(pair.Key))
                    .GroupBy(pair => pair.Value)
                    .ToDictionary(grouping => grouping.Key, grouping => grouping.Select(pair => pair.Key).ToList());
            UiControllers =
                TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSUIController>()
                    .FindAll(type => !AppUiControllers.SelectMany(pair => pair.Value).Contains(type));
            AdminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSAdminController>()
                .FindAll(type => !AppAdminControllers.SelectMany(pair => pair.Value).Contains(type));
        }

        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            string appName = Convert.ToString(requestContext.RouteData.DataTokens["app"]);
            string areaName = Convert.ToString(requestContext.RouteData.DataTokens["area"]);

            var listToCheck = "admin".Equals(areaName, StringComparison.OrdinalIgnoreCase)
                ? GetAdminControllersToCheck(appName)
                : GetUiControllersToCheck(appName);

            Type controllerType =
                listToCheck.FirstOrDefault(
                    type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            return controllerType;
        }

        private List<Type> GetAdminControllersToCheck(string appName)
        {
            var types = new List<Type>();
            if (!String.IsNullOrWhiteSpace(appName) && AppAdminControllers.ContainsKey(appName))
                types.AddRange(AppAdminControllers[appName]);
            types.AddRange(AdminControllers);
            foreach (
                var key in
                    AppAdminControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase))
                )
            {
                types.AddRange(AppAdminControllers[key]);
            }
            return types;
        }

        private List<Type> GetUiControllersToCheck(string appName)
        {
            var types = new List<Type>();
            if (!String.IsNullOrWhiteSpace(appName) && AppUiControllers.ContainsKey(appName))
                types.AddRange(AppUiControllers[appName]);
            types.AddRange(UiControllers);
            foreach (
                var key in
                    AppUiControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase)))
            {
                types.AddRange(AppUiControllers[key]);
            }
            return types;
        }

        public bool IsValidControllerType(string appName, string controllerName, bool isAdmin)
        {
            string typeName = controllerName + "Controller";
            if (!String.IsNullOrWhiteSpace(appName))
            {
                return isAdmin
                    ? AppAdminControllers.ContainsKey(appName) && AppAdminControllers[appName].Any(
                        type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                    : AppUiControllers.ContainsKey(appName) && AppUiControllers[appName].Any(
                        type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            }
            return isAdmin
                ? AdminControllers.Any(
                    type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                : UiControllers.Any(
                    type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        }

        public static IEnumerable<MethodInfo> GetActionMethods(Type controllerType)
        {
            return controllerType.GetMethods()
                .Where(
                    q =>
                        q.IsPublic &&
                        (typeof (ActionResult).IsAssignableFrom(q.ReturnType) ||
                         (q.ReturnType.IsGenericType && q.ReturnType.GetGenericTypeDefinition() == typeof (Task<>) &&
                          q.ReturnType.GetGenericArguments().Count() == 1 &&
                          typeof (ActionResult).IsAssignableFrom(q.ReturnType.GetGenericArguments()[0]))));
        }

        public static List<ActionMethodInfo<T>> GetActionMethodsWithAttribute<T>() where T : Attribute
        {
            var reflectedControllerDescriptors = AllControllers.Select(type => new ReflectedControllerDescriptor(type));
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

    public class ActionMethodInfo<T>
    {
        public ActionDescriptor Descriptor { get; set; }
        public T Attribute { get; set; }
    }
}