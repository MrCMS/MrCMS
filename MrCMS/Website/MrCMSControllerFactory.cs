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
        private Dictionary<string, List<Type>> _appUiControllers;
        private Dictionary<string, List<Type>> _appAdminControllers;
        private List<Type> _uiControllers;
        private List<Type> _adminControllers;

        public MrCMSControllerFactory()
        {
            _appUiControllers =
                TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(MrCMSAppUIController<>))
                          .GroupBy(
                              type =>
                              ((MrCMSApp)Activator.CreateInstance(
                                  TypeHelper.GetBaseTypes(type, type1 =>
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