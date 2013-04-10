using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class AppRouteConstraint : IRouteConstraint
    {
        private readonly string _appName;
        private readonly string _area;

        public AppRouteConstraint(string appName, string area)
        {
            _appName = appName;
            _area = area;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var mrCMSControllerFactory = ControllerBuilder.Current.GetControllerFactory() as MrCMSControllerFactory;
            if (mrCMSControllerFactory != null)
            {
                return mrCMSControllerFactory.IsValidControllerType(_appName, values["controller"].ToString(),
                                                                    !string.IsNullOrWhiteSpace(_area) &&
                                                                    _area.Equals("Admin",
                                                                                 StringComparison.OrdinalIgnoreCase));
            }
            return false;
        }
    }
}