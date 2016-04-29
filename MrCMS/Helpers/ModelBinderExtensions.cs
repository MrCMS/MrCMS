using System;
using System.Web.Mvc;

namespace MrCMS.Helpers
{
    public static class ModelBinderExtensions
    {
        public static string GetValueFromRequest(this ControllerContext controllerContext, string request)
        {
            return controllerContext.HttpContext.Request[request];
        }
        public static string GetValueFromRouteData(this ControllerContext controllerContext, string request)
        {
            return Convert.ToString(controllerContext.RouteData.Values[request]);
        }

    }
}