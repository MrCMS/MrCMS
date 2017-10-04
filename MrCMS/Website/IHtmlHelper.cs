using System.Web.Mvc;
using System.Web.Routing;

namespace MrCMS.Website
{
    public interface IHtmlHelper
    {
        void RenderAction(string actionName);
        void RenderAction(string actionName, object routeValues);
        void RenderAction(string actionName, RouteValueDictionary routeValues);
        void RenderAction(string actionName, string controllerName);
        void RenderAction(string actionName, string controllerName, object routeValues);
        void RenderAction(string actionName, string controllerName, RouteValueDictionary routeValues);

        MvcHtmlString Action(string actionName);
        MvcHtmlString Action(string actionName, object routeValues);
        MvcHtmlString Action(string actionName, RouteValueDictionary routeValues);
        MvcHtmlString Action(string actionName, string controllerName);
        MvcHtmlString Action(string actionName, string controllerName, object routeValues);
        MvcHtmlString Action(string actionName, string controllerName, RouteValueDictionary routeValues);



        void RenderPartial(string viewName);
        ViewContext ViewContext { get; }
    }
}