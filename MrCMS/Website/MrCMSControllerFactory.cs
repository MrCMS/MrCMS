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
        private readonly Dictionary<string, List<Type>> _appUiControllers;
        private readonly Dictionary<string, List<Type>> _appAdminControllers;
        private readonly List<Type> _uiControllers;
        private readonly List<Type> _adminControllers;

        public MrCMSControllerFactory()
        {
            _appUiControllers =
                TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(MrCMSAppUIController<>))
                          .GroupBy(
                              type =>
                              ((MrCMSApp)Activator.CreateInstance(
                                  type.GetBaseTypes(type1 =>
                                            type1.IsGenericType &&
                                            type1.GetGenericTypeDefinition() == typeof(MrCMSAppUIController<>))
                                      .First()
                                      .GetGenericArguments()[0])).AppName)
                          .ToDictionary(types => types.Key, types => types.ToList());
            _appAdminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(MrCMSAppAdminController<>))
                                             .GroupBy(
                                                 type =>
                                                 ((MrCMSApp)Activator.CreateInstance(
                                                     type.GetBaseTypes(
                                                         type1 =>
                                                         type1.IsGenericType &&
                                                         type1.GetGenericTypeDefinition() ==
                                                         typeof(MrCMSAppAdminController<>))
                                                         .First()
                                                         .GetGenericArguments()[0])).AppName)
                                             .ToDictionary(types => types.Key, types => types.ToList());
            _uiControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSUIController>();
            _adminControllers = TypeHelper.GetAllConcreteTypesAssignableFrom<MrCMSAdminController>();
        }
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            Type controllerType = null;
            if (_appUiControllers.ContainsKey(Convert.ToString(requestContext.RouteData.DataTokens["app"])))
            {
                if ("admin".Equals(Convert.ToString(requestContext.RouteData.DataTokens["area"]),
                                   StringComparison.OrdinalIgnoreCase))
                {
                    controllerType = _appAdminControllers[requestContext.RouteData.DataTokens["app"].ToString()].SingleOrDefault(
                        type =>
                        type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
                }
                else
                    controllerType = _appUiControllers[requestContext.RouteData.DataTokens["app"].ToString()].SingleOrDefault(
                        type =>
                        type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            }
            if (controllerType == null)
            {
                if ("admin".Equals(Convert.ToString(requestContext.RouteData.DataTokens["area"]),
                                   StringComparison.OrdinalIgnoreCase))
                    controllerType = _adminControllers.SingleOrDefault(
                        type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
                else
                    controllerType = _uiControllers.SingleOrDefault(
                        type => type.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase));
            }
            return controllerType;
        }

        public bool IsValidControllerType(string appName, string controllerName, bool isAdmin)
        {
            string typeName = controllerName + "Controller";
            if (!string.IsNullOrWhiteSpace(appName))
            {
                return isAdmin
                           ? _appAdminControllers.ContainsKey(appName) && _appAdminControllers[appName].Any(
                               type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                           : _appUiControllers.ContainsKey(appName) && _appUiControllers[appName].Any(
                               type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            }
            return isAdmin
                       ? _adminControllers.Any(
                           type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase))
                       : _uiControllers.Any(
                           type => type.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        }
    }
}