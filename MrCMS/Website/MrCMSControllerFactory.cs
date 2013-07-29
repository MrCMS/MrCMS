using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Apps;
using MrCMS.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Website
{
    public class MrCMSControllerFactory : DefaultControllerFactory
    {
        public static readonly Dictionary<string, List<Type>> AppUiControllers;
        public static readonly Dictionary<string, List<Type>> AppAdminControllers;
        public static readonly List<Type> UiControllers;
        public static readonly List<Type> AdminControllers;

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

            Type controllerType = listToCheck.FirstOrDefault(type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            return controllerType;
        }

        private List<Type> GetAdminControllersToCheck(string appName)
        {
            var types = new List<Type>();
            if (!string.IsNullOrWhiteSpace(appName))
                types.AddRange(AppAdminControllers[appName]);
            types.AddRange(AdminControllers);
            foreach (var key in AppAdminControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase)))
            {
                types.AddRange(AppAdminControllers[key]);
            }
            return types;
        }

        private List<Type> GetUiControllersToCheck(string appName)
        {
            var types = new List<Type>();
            if (!string.IsNullOrWhiteSpace(appName))
                types.AddRange(AppUiControllers[appName]);
            types.AddRange(UiControllers);
            foreach (var key in AppUiControllers.Keys.Where(s => !s.Equals(appName, StringComparison.InvariantCultureIgnoreCase)))
            {
                types.AddRange(AppUiControllers[key]);
            }
            return types;
        }

        public bool IsValidControllerType(string appName, string controllerName, bool isAdmin)
        {
            string typeName = controllerName + "Controller";
            if (!string.IsNullOrWhiteSpace(appName))
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
    }
}